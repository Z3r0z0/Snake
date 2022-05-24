using System.Security.Cryptography;
using System.Text;

namespace Snake.Extensions
{
    public static class StringExtension
    {
        public static byte[] HashString(this string text)
        {
            using var sha = SHA256.Create();

            return sha.ComputeHash(Encoding.UTF8.GetBytes(text + "poqjefpoafdöla"));
        }
    }
}
