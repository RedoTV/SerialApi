using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SerialApi.HelperClasses
{
    public class AuthOptions
    {
        public const string ISSUER = "Zinovich"; // издатель токена
        public const string AUDIENCE = "Zozna"; // потребитель токена
        const string KEY = "JoJo_Is_The_Best_Anime";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
