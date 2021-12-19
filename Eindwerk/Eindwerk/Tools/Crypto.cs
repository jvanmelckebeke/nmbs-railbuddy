using System.Security.Cryptography;
using System.Text;

namespace Eindwerk.Tools
{
    public static class Crypto
    {
        /**
         * <summary>base method to generate a hash from any HashAlgorithm</summary>
         */
        private static string GenerateHash(string plainString, HashAlgorithm hash)
        {
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainString);
            byte[] hashBytes = hash.ComputeHash(plainBytes);

            var builder = new StringBuilder();
            foreach (byte b in hashBytes) builder.Append(b.ToString("x2"));

            return builder.ToString();
        }

        /**
         * <summary>computes MD5 hash of a string</summary>
         * <remark>used for gravatar</remark>
         */
        public static string ComputeMd5(string rawString)
        {
            using (var md5 = MD5.Create())
            {
                return GenerateHash(rawString, md5);
            }
        }
    }
}