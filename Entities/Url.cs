using System.ComponentModel.DataAnnotations.Schema;
using Base;

namespace Entities;

[Table(name:"Urls", Schema = "Shortener")]
public class Url : Entity<long>
{
   [Column(TypeName = "varchar(255)")]
   public required string ShortUrl {get; set;}

   [Column(TypeName = "varchar(255)")]
   public required string OriginalUrl {get; set;}

   public required DateTimeOffset Expiration {get; set;} //DateTime sadece saati tutar ama biz burda farklı coğrafyalardan gelen verileri işliyoruz o yüzden DateTimeOffSet kullanılmalı
                                                         //örneğin sadece Türkiye'de çalışan bir masaüstü uygulaması için DateTime yeterli olabilir.

   public required DateTimeOffset  Created {get; set;}
   
   public virtual ICollection<Analytic>? Analytics {get; set;}
}

