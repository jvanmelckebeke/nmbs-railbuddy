using System;
using System.Collections.Generic;
using Eindwerk.Models.BuddyApi.Friends;
using Eindwerk.Tools;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Eindwerk.Models.BuddyApi
{
    public class UserProfile : IDtoModel
    {
        [JsonProperty(PropertyName = "id")] public Guid ProfileId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")] public string Email { get; set; }

        [JsonProperty(PropertyName = "targetCity")]
        public string TargetCity { get; set; }

        [JsonProperty("friends")] public List<Friend> Friends { get; set; } = new List<Friend>();

        [JsonProperty("friendRequestReceived")]
        public List<FriendRequest> FriendRequestsReceived { get; set; } = new List<FriendRequest>();


        [JsonProperty("friendRequestSent")]
        public List<FriendRequest> FriendRequestsSent { get; set; } = new List<FriendRequest>();

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

        public ImageSource Avatar
        {
            get
            {
                var avatarUrl = IsFilled()
                    ? AvatarUrl
                    : "https://www.gravatar.com/avatar/00000000000000000000000000000000?d=wavatar&s=200&qzone=2";

                return ImageSource.FromUri(new Uri(avatarUrl));
            }
        }

        public string QrCodeUrl =>
            $"https://api.qrserver.com/v1/create-qr-code/?data={ProfileId.ToString()}&size=300x300&ecc=M";

        public bool IsFilled()
        {
            return !(Email.IsNullOrEmpty() || Username.IsNullOrEmpty());
        }


        public override string ToString()
        {
            return $"{nameof(UserProfile)}[{nameof(ProfileId)}: {ProfileId}, " +
                   $"{nameof(Username)}: {Username}, " +
                   $"{nameof(Email)}: {Email}, " +
                   $"{nameof(QrCodeUrl)}: {QrCodeUrl}, " +
                   $"{nameof(AvatarUrl)}: {AvatarUrl}, " +
                   $"{nameof(TargetCity)}: {TargetCity}, " +
                   $"{nameof(Friends)}: {Friends.Count} friends, " +
                   $"{nameof(FriendRequestsReceived)}: {FriendRequestsReceived.Count} requests, " +
                   $"{nameof(FriendRequestsSent)}: {FriendRequestsSent.Count} requests]";
        }

        #region equality

        protected bool Equals(UserProfile other)
        {
            return ProfileId.Equals(other.ProfileId) && Username == other.Username && Email == other.Email &&
                   TargetCity == other.TargetCity && Equals(Friends, other.Friends) &&
                   Equals(FriendRequestsReceived, other.FriendRequestsReceived) &&
                   Equals(FriendRequestsSent, other.FriendRequestsSent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((UserProfile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ProfileId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TargetCity != null ? TargetCity.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Friends != null ? Friends.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (FriendRequestsReceived != null ? FriendRequestsReceived.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FriendRequestsSent != null ? FriendRequestsSent.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(UserProfile left, UserProfile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserProfile left, UserProfile right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}