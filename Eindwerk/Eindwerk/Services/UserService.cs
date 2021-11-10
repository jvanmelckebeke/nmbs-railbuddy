using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Repository;
using Eindwerk.Tools;

namespace Eindwerk.Services
{
    public class UserService : SecuredService
    {
        private UserRepository _userRepository;

        private string _ownProfileId;


        public UserService(string accessToken) : base(accessToken)
        {
        }

        public async Task<UserProfile> GetUserProfileAsync(string profileId = null)
        {
            profileId = profileId ?? _ownProfileId;

            return await _userRepository.GetUserProfileAsync(profileId);
        }

        protected override void SetupAfterTokenSet()
        {
            _userRepository = new UserRepository(AccessToken);
            _ownProfileId = JwtTokenTools.ExtractTokenSubject(AccessToken);
        }
    }
}