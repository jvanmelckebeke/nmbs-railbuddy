using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.BuddyApi.Friends;
using Eindwerk.Models.Rail;
using Eindwerk.Repository;
using Eindwerk.Tools;

namespace Eindwerk.Services
{
    public class UserService : SecuredService
    {
        private UserRepository _userRepository;

        private string      _ownProfileId;
        private UserProfile _ownProfile;


        public UserService(string accessToken) : base(accessToken) { }

        public async Task<UserProfile> GetUserProfileAsync(string profileId = null)
        {
            profileId = profileId ?? _ownProfileId;

            return await _userRepository.GetUserProfileAsync(profileId);
        }

        protected override async void SetupAfterTokenSet()
        {
            _userRepository = new UserRepository(AccessToken);
            _ownProfileId = JwtTokenTools.ExtractTokenSubject(AccessToken);
            await SetOwnUserProfileAsync();
        }


        private async Task SetOwnUserProfileAsync()
        {
            _ownProfile = await GetUserProfileAsync(_ownProfileId);
        }

        public async Task<UserProfile> GetOwnUserProfileAsync()
        {
            return _ownProfile ?? await GetUserProfileAsync();
        }

        public async Task<List<UserProfile>> GetFriendsAsync(string profileId = null)
        {
            if (profileId == null)
            {
                profileId = _ownProfileId;
            }

            List<UserProfile> friends = await _userRepository.GetFriendsAsync(profileId);

            return friends;
        }

        public async Task<BasicFriendRequestStatus> RequestFriendAsync(string friendId)
        {
            return await _userRepository.DoFriendAction(friendId, FriendAction.Request);
        }


        public async Task<BasicFriendRequestStatus> AcceptFriendAsync(string friendId)
        {
            return await _userRepository.DoFriendAction(friendId, FriendAction.Accept);
        }

        public async Task<BasicFriendRequestStatus> DeleteFriendAsync(string friendId)
        {
            return await _userRepository.DoFriendAction(friendId, FriendAction.Delete);
        }

        public async Task<SeatRegistration> RegisterSeat(SeatRegistration seat)
        {
            return await _userRepository.RegisterSeat(seat);
        }

        public async Task<List<Wagon>> GetTrainCompositionAsync(string vehicleNumber)
        {
            var beneluxTrainsRepository = new BeneluxTrainsRepository();

            Debug.WriteLine($"searching for friends on train with number {vehicleNumber}");

            List<FriendSeatRegistration> friendsSeats = await _userRepository.GetFriendsOnTrainAsync(vehicleNumber);
            SeatRegistration ownSeat = await _userRepository.GetOwnSeatRegistrationAsync();

            List<Wagon> wagons = await beneluxTrainsRepository.GetTrainCompositionAsync(vehicleNumber);

            if (ownSeat != null && wagons != null && wagons.Count > ownSeat.WagonIndex)
            {
                if (_ownProfile == null)
                {
                    await SetOwnUserProfileAsync();
                }

                Debug.WriteLine($"own user profile: {_ownProfile}");
                
                var own = new Friend
                {
                    Email = _ownProfile.Email,
                    UserId = _ownProfile.ProfileId,
                    Username = "me"
                };
                wagons[ownSeat.WagonIndex].FriendsInWagon.Add(own);
            }

            foreach (FriendSeatRegistration friendSeatRegistration in friendsSeats)
            {
                if (wagons.Count > friendSeatRegistration.SeatRegistration.WagonIndex)
                {
                    wagons[friendSeatRegistration.SeatRegistration.WagonIndex].FriendsInWagon
                                                                              .Add(friendSeatRegistration.Friend);
                }
            }

            return wagons;
        }

        public async Task<SeatRegistration> GetOwnSeatRegistrationAsync()
        {
            return await _userRepository.GetOwnSeatRegistrationAsync();
        }

        public async Task RemoveSeat()
        {
            await _userRepository.RemoveSeat();
        }
    }
}