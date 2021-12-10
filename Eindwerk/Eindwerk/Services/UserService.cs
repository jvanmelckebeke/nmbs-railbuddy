﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Eindwerk.Models;
using Eindwerk.Models.BuddyApi;
using Eindwerk.Models.BuddyApi.Friends;
using Eindwerk.Repository;
using Eindwerk.Tools;

namespace Eindwerk.Services
{
    public class UserService : SecuredService
    {
        private UserRepository _userRepository;

        private string _ownProfileId;
        private UserProfile _ownProfile;


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
            SetOwnUserProfileAsync();
        }


        private async void SetOwnUserProfileAsync()
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

        public async Task<FriendRequest> RequestFriendAsync(string friendId)
        {
            return await _userRepository.DoFriendAction(friendId, FriendAction.Request);
        }


        public async Task<FriendRequest> AcceptFriendAsync(string friendId)
        {
            return await _userRepository.DoFriendAction(friendId, FriendAction.Accept);
        }

        public async Task<FriendRequest> DeleteFriendAsync(string friendId)
        {
            return await _userRepository.DoFriendAction(friendId, FriendAction.Delete);
        }
    }
}