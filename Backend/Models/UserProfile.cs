using System;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Models
{
    public class UserProfile
    {
        public Guid ProfileId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string AvatarUrl
        {
            get
            {
                // it is quite ridiculous how much work you should do to just generate a md5 hash of a string
                var emailLowercase = Email.ToLower().Trim();
                var emailBytes = Encoding.ASCII.GetBytes(emailLowercase);

                using (MD5 hash = MD5.Create())
                {
                    var hashBytes = hash.ComputeHash(emailBytes);

                    var sb = new StringBuilder();
                    foreach (var b in hashBytes)
                    {
                        sb.Append(b.ToString("X2"));
                    }

                    var emailHash = sb.ToString();

                    return $"https://www.gravatar.com/avatar/{emailHash}?d=wavatar";
                }
            }
        }
    }
}