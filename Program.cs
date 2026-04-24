using Context;
using Options;
using Helpers;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("LocalDb");
});

builder.Services.AddHttpContextAccessor();
//Bu satır, HTTP isteğine dair bilgilere (kullanıcı bilgisi, header'lar, session vb)servis katmanından veya herhangi bir sınıftan erişmenizi sağlar.


var shortenerOption = builder.Configuration.GetSection("Shortener").Get<ShortenerOption>();
//Bu satır, appsettings.json dosyanızdaki yapılandırma verilerini okur ve bir C# nesnesine dönüştürür.

builder.Services.AddSingleton(shortenerOption);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();



//POST link'i create etcek 

app.MapPost("/link/add",([FromBody] string url,[FromServices] ApplicationDbContext context)=>
{
    if (string.IsNullOrEmpty(url))
    {
        throw new ValidationException("Url cannot be empty");
    }

    var options = app.Services.GetRequiredService<ShortenerOption>();
    
    //var code = CodeGenerator.GeneratorCode(options.ShortUrlLength,options.ShortUrlCharacters);
    
    string code; // kodu döngü dışında kullanabilmek için burada tanımlıyoruz
    //TODO: chech if code is unique
    while (true)
    {
        //1.önce bir kod üretiyoruz
        code = CodeGenerator.GeneratorCode(options.ShortUrlLength,options.ShortUrlCharacters);
        //2.veritabanında bu kodla biten bir kayıt var mı diye bakıyoruz 
        var isExist = context.Url.Any(x => x.ShortUrl.EndsWith(code)); //.FirstOrDefault() yerine .Any() kullandık.FirstOrDefault tüm veriyi getirmeye çalışır. Any ise sadece "Var mı yok mu?" diye bakar ve true/false döner.
        //3.Eğer yoksa döngüden çık
        if (isExist == false)
        {
            break;
        }
        //eğer varsa döngü başa döner ve yeni bir kod üretir
    }

    var shortUrl = $"{options.BaseUrl}/{code}";
    //Ana site adresi ile üretilen kodu birleştirir. (Örn: mysite.com/xR4z9)
    var expDate = DateTimeOffset.Now.AddMinutes(options.ShortUrlExpiration);
    //Linkin ömrünü belirler. Şu anki zamana, ayarlardaki dakika kadar ekleme yapar
    
    //create a entity
    var entity = new Url
    {
        ShortUrl = shortUrl,
        OriginalUrl = url,
        Expiration = expDate,
        Created = DateTimeOffset.Now
    };
    context.Url.Add(entity);
    context.SaveChanges();

    return Results.Ok(new
    {
        shortUrl,
        expDate
    });
    //API'nin kullanıcıya verdiği son cevaptır


});

// longUrl'u getircek 
app.MapGet("{code}",(string code,[FromServices] ApplicationDbContext context) =>
{
    var options = app.Services.GetRequiredService<ShortenerOption>();

    var shortUrl = $"{options.BaseUrl}/{code}";

    // Gelen shortUrl'yi database de arıcak, eğer bulursa o satırdaki Original Url'yi çekicek
    var entity = context.Url
        //Buradaki metodlar aslında veri tabanına gönderdiğin sorgu emirleridir
          .AsQueryable() //1 milyon linkin olsa bile hepsini okumaz, sadece senin istediğini bulmaya odaklanır.
          .AsNoTracking() //Entity Framework, veri tabanından çektiği her nesneyi "acaba üzerinde bir değişiklik yapacak mı?" diye gizlice izler.biz burada sadece linki okuyoruz, üzerinde bir güncelleme yapmayacağız. "Beni boşuna izleyip yorulma" diyerek sistemi hızlandırıyoruz. Profesyonel projelerde "Sadece Okuma" (Read-only) işlemlerinde bu mutlaka kullanılır.
          .FirstOrDefault(x => x.ShortUrl == shortUrl);

    if(entity is null)
    {
        return Results.NotFound("Link not found.");
    }

    //analitik kayıdı oluştur
    var contextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

    var analytic = new Analytic
    {
        UrlId = entity.Id,
        UserAgent = contextAccessor?.HttpContext?.Request.Headers.UserAgent,
        IpAddress = contextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString(),
        Referer = contextAccessor?.HttpContext?.Request.Headers.Referer,
        Created = DateTimeOffset.Now

    };

    context.Analytic.Add(analytic);
    context.SaveChanges();
    return Results.Redirect(entity.OriginalUrl);
    
});

//analitik kısımdaki verileri götürcek
app.MapGet("list/{code}/analyze",(string code , [FromServices] ApplicationDbContext context )=>
{
    var options = app.Services.GetRequiredService<ShortenerOption>();

    var shortUrl = $"{options.BaseUrl}/{code}";

    var entity = context.Url
             .AsQueryable()
             .AsTracking()
             .Include(x => x.Analytics) // ekrana analitik verilerinide yazdırcaz o yüzden aldığımız verileri databaseden çekiyoruz 
             .FirstOrDefault(x => x.ShortUrl == shortUrl);

    if (entity is null)
    {
        return Results.NotFound("Link not found");
    }

    return Results.Ok(new
    {
        entity.OriginalUrl,
        entity.Expiration,
        entity.Created,
        Analytic = entity.Analytics?.Select( x => new
        {
            x.UserAgent,
            x.IpAddress,
            x.Referer,
            x.Created
            
        })
        
    });

});

app.Run();


