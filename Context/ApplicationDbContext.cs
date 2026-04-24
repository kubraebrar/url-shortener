using System.Diagnostics.Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Context;

//public class ApplicationDbContext : DbContext
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext
{
   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
    optionsBuilder.UseInMemoryDatabase("LocalDb");
   }
   
   //Db ye bu tabloları eklememiz gerekiyor 
   public DbSet<Analytic> Analytic {get; set;}
   public DbSet<Url> Url {get; set;}
   
}