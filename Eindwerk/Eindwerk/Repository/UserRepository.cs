using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.BuddyApi.Friends;

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

        public async Task<BasicFriendRequestStatus> DoFriendAction(string profileId, FriendAction action)
        {
            BasicFriendRequestStatus ret = await DoPutRequest<BasicFriendRequestStatus>($"{BASEURI}/user/{profileId}/friend",
                new FriendRequestAction(action));

            return ret;
        }
    }
}