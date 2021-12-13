using System;
using Newtonsoft.Json;

namespace Backend.Domain
{
    public class Friend
    {
        public Friend()
        {
        }

        public Friend(UserProfile profile)
        {
            UserId = profile.ProfileId;
            Email = profile.Email;
            Username = profile.Username;
        }

        [JsonProperty("userId")] public Guid UserId { get; set; }

        [JsonProperty("email")] public string Email { get; set; }

        [JsonProperty("username")] public string Username { get; set; }

        public override string ToString()
        {
            return $"Friend[{nameof(UserId)}: {UserId}, {nameof(Email)}: {Email}, {nameof(Username)}: {Username}]";
        }

        protected bool Equals(Friend other)
        {
            return UserId.Equals(other.UserId) && Email == other.Email && Username == other.Username;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((Friend) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, Email, Username);
        }

        public static bool operator ==(Friend left, Friend right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Friend left, Friend right)
        {
            return !Equals(left, right);
        }
    }
}