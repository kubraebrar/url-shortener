namespace Helpers;

public static class CodeGenerator
{
    public static string GeneratorCode(int length, string chars)
    {
        var random = new Random();
        var result ="";
        for (int i=0; i <length; i++)
        {
            result += chars[random.Next(chars.Length)];
        }
        return result;
        // LINQ return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
 
    }
}