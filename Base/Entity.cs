using System.ComponentModel.DataAnnotations;
namespace Base;

public class Entity<TKey> //Buna Generic Class denir.Buradaki <TKey> "ben buraya her türlü kimlik tipi ID tipi koyabilirim demektir.
                          // Bazı tabloların ID si int bazılarının string olur, TKey kullanarak hangi tipte ID istersen onu kullanabilirsin demiş oluyosun.
    where TKey : struct //TKey olarak adlandırılan tip parametresinin değer türü (value type) olmak zorunda olduğunu belirten bir generic kısıtlamadır (constraint). 
                        //TKey yerine kullanılacak tipin bir class (referans türü) değil, struct (değer türü) olmasını garanti etmektir. 
{
    [Key] // Bu bir Attribute (Öznitelik)'tir. Entity Framework'e şu mesajı verir "Bak, altındaki bu Id değişkeni var ya, işte o bu tablonun Primary Key'idir (yani her satırı birbirinden ayıran benzersiz kimliktir)."
   public TKey Id {get; set;} //Bu da her tablonun ortak sahip olacağı ID kolonudur.
}