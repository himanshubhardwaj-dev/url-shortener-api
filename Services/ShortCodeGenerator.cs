using System.Security.Cryptography;
using URLShortner.API.Interfaces;

namespace URLShortner.API.Services
{
    public class ShortCodeGenerator : IShortCodeGenerator
    {
        private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public string Generate()
        {
            char[] result = new char[6];
            for(int i = 0; i < 6; i++)
            {
                int index = RandomNumberGenerator.GetInt32(Characters.Length);
                result[i] = Characters[index];
            }
            return new string(result); // Suppose FI0YTQ
        }
    }
}
