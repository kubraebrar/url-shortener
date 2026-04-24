using System.ComponentModel.DataAnnotations.Schema;
using Base;

namespace Entities;

[Table(name:"Analytic", Schema = "Shortener")]
public class Analytic : Entity<long>
{
   public long UrlId {get; set;}

   [ForeignKey(nameof(UrlId))]
   public Url? Url {get; set;}

   public DateTimeOffset Created {get; set;}

   public string? UserAgent {get; set;} //? işareti: UserAgent özelliğinin (property) null (boş) bir değer alabileceğini ifade eder. 
   public string? IpAddress {get; set;}
   public string? Referer {get; set;}
   public string? Country {get; set;}
   public string? Region {get; set;}
   public string? City {get; set;}
   public string? Browser {get; set;}
   public string? Os {get; set;}
}