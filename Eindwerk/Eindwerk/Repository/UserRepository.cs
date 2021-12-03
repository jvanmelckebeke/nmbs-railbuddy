using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;

namespace Eindwerk.Repository
{
    public class UserRepository : RailBuddyApiRepository
    {
        public UserRepository(string accessToken) : base(accessToken)
        {
        }

        public Task<UserProfile> GetUserProfileAsync(string profileId)
        {
            return DoGetRequest<UserProfile>($"{BASEURI}/user/{profileId}");
        }

        public async Task<List<UserProfile>> GetFriendsAsync(string profileId)
        {
            DtoList<UserProfile> ret = await DoGetRequest<DtoList<UserProfile>>($"{BASEURI}/user/{profileId}/friends");

            Debug.WriteLine("got friends");
            return ret == null ? new List<UserProfile>() : ret.ToList();
        }

        public async Task<FriendRequest> AddFriendAsync(string profileId)
        {
            FriendRequest ret = await DoPutRequest<FriendRequest>($"{BASEURI}/user/{profileId}/friend", new
            {
                Action = "Request"
            });

            return ret;
        }
    }
}