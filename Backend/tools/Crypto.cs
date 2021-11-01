using System.Security.Cryptography;
using System.Text;

namespace Backend.tools
{
    public static class Crypto
    {
        /**
         * <summary>base method to generate a hash from any HashAlgorithm</summary>
         */
        private static string GenerateHash(string plainString, HashAlgorithm hash)
        {
            var plainBytes = Encoding.ASCII.GetBytes(plainString);
            var hashBytes = hash.ComputeHash(plainBytes);

            var builder = new StringBuilder();
            foreach (var b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

        /**
         * <summary>computes SHA256 hash of a string</summary>
         *
         * <remark>used for passwords</remark>
         */
        public static string ComputeSha256(string rawString)
        {
            using var sha256 = SHA256.Create();
            return GenerateHash(rawString, sha256);
        }

        /**
         * <summary>computes MD5 hash of a string</summary>
         *
         * <remark>used for gravatar</remark>
         */
        public static string ComputeMd5(string rawString)
        {
            using var md5 = MD5.Create();
            return GenerateHash(rawString, md5);
        }
    }
}