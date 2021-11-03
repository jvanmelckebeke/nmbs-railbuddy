using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Repository;

namespace Eindwerk.Services
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
    }
}