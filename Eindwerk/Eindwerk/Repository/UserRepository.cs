using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.BuddyApi.Friends;
using Xamarin.Forms;

namespace Eindwerk.Repository
{
    public class UserRepository : RailBuddyApiRepository
    {
        const bool DEBUG_USER = false;

        public UserRepository(string accessToken) : base(accessToken) { }

        public Task<UserProfile> GetUserProfileAsync(string profileId)
        {
            return DoGetRequest<UserProfile>($"{BASEURI}/user/{profileId}", DEBUG_USER);
        }

        public async Task<List<UserProfile>> GetFriendsAsync(string profileId)
        {
            DtoList<UserProfile> ret =
                await DoGetRequest<DtoList<UserProfile>>($"{BASEURI}/user/{profileId}/friends", DEBUG_USER);

            Debug.WriteLine("got friends");
            return ret == null ? new List<UserProfile>() : ret.ToList();
        }

        public async Task<BasicFriendRequestStatus> DoFriendAction(string profileId, FriendAction action)
        {
            BasicFriendRequestStatus ret = await DoPutRequest<BasicFriendRequestStatus>(
                $"{BASEURI}/user/{profileId}/friend", new FriendRequestAction(action), DEBUG_USER);

            return ret;
        }

        public async Task<SeatRegistration> RegisterSeat(SeatRegistration seat)
        {
            return await DoPostRequest<SeatRegistration>($"{BASEURI}/line", seat, DEBUG_USER);
        }

        public async Task<List<FriendSeatRegistration>> GetFriendsOnTrainAsync(string vehicleNumber)
        {
            return await DoGetRequest<DtoList<FriendSeatRegistration>>($"{BASEURI}/line/{vehicleNumber}", true);
        }
    }
}