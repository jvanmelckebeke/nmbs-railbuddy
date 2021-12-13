using System;
using Eindwerk.Tools;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Eindwerk.Models.BuddyApi.Friends
{
    public class Friend : IDtoModel
    {
        public Friend()
        {
        }

        public Friend(Friend other)
        {
            this.UserId = other.UserId;
            this.Email = other.Email;
            this.Username = other.Username;
        }

        [JsonProperty("userId")] public Guid UserId { get; set; }

        [JsonProperty("email")] public string Email { get; set; }

        [JsonProperty("username")] public string Username { get; set; }

        /// <summary>
        /// generated Gravatar url
        /// </summary>
        public string AvatarUrl
        {
            get
            {
                var emailLowercase = Email.ToLower().Trim();
                var emailHash = Crypto.ComputeMd5(emailLowercase);

                return $"https://www.gravatar.com/avatar/{emailHash}?d=wavatar&s=200&qzone=2";
            }
        }

        public ImageSource Avatar => ImageSource.FromUri(new Uri(AvatarUrl));

        public override string ToString()
        {
            return $"Friend[{nameof(UserId)}: {UserId}, {nameof(Email)}: {Email}, {nameof(Username)}: {Username}]";
        }

        public bool IsFilled()
        {
            return Email != null;
        }

        #region equality

        protected bool Equals(Friend other)
        {
            return UserId.Equals(other.UserId) && Email == other.Email && Username == other.Username;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Friend) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = UserId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Friend left, Friend right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Friend left, Friend right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}