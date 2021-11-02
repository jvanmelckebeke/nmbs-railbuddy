using System.Threading.Tasks;
using Eindwerk.Models;

namespace Eindwerk.Services
{
    public class UserService : ApiService
    {

        public UserService(string accessToken) : base(accessToken)
        {
        }

        public Task<UserProfile> GetUserProfile(string profileId)
        {
            return DoGetRequest<UserProfile>($"{BASEURI}/user/{profileId}");
        }
    }
}