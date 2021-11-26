using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.dto;
using Backend.exceptions;
using Backend.repositories;
using Backend.tools;
using FriendRequest = Backend.Domain.FriendRequest;

namespace Backend.services
{
    public static class UserService
    {
        private static UserProfileResponse ConvertToResponse(UserProfile source)
        {
            return ClassMapping.ConvertDomainDto<UserProfile, UserProfileResponse>(source);
        }

        public static async Task<UserProfileResponse> CreateProfileAsync(UserProfile profile)
        {
            // prepare profile

            profile.ProfileId = Guid.NewGuid();

            profile.Password = Crypto.ComputeSha256(profile.Password); // encrypt the password

            profile.TargetCity ??= "UNKNOWN";
            // basically, if targetCity is null, then set it to UNKNOWN,
            // also this is probably gonna bite me in the ass later on


            return ConvertToResponse(await UserRepository.CreateProfileAsync(profile));
        }


        public static Task<UserProfileResponse> GetProfileByProfileIdAsync(string profileGuid)
        {
            return GetProfileByProfileIdAsync(Guid.Parse(profileGuid));
        }

        public static async Task<UserProfileResponse> GetProfileByProfileIdAsync(Guid profileGuid)
        {
            return ConvertToResponse(await UserRepository.FindOneByProfileIdAsync(profileGuid));
        }

        public static async Task<List<UserProfileResponse>> GetFriendsAsync(string profileId)
        {
            Guid profileGuid = Guid.Parse(profileId);

            UserProfile profile = await UserRepository.FindOneByProfileIdAsync(profileGuid);

            List<UserProfileResponse> ret = new List<UserProfileResponse>();

            foreach (var friendId in profile.Friends)
            {
                ret.Add(await GetProfileByProfileIdAsync(friendId));
            }

            return ret;
        }

        public static FriendRequestStatus GetFriendRequestStatus(UserProfile currentProfile,
            string friendId)
        {
            Guid friendGuid = Guid.Parse(friendId);

            if (currentProfile.Friends.Any(friend => friend == friendGuid))
            {
                return FriendRequestStatus.Accepted;
            }

            foreach (FriendRequest request in currentProfile.FriendRequestsSent.Where(request =>
                request.UserId == friendGuid))
            {
                return request.FriendRequestStatus;
            }

            return currentProfile.FriendRequestsReceived
                .Where(request => request.UserId == friendGuid)
                .Select(request => request.FriendRequestStatus)
                .FirstOrDefault();
        }

        public static async Task<FriendRequestResponse> CreateFriendRequestAsync(UserProfile currentProfile,
            string toProfileId)
        {
            Guid toProfileGuid = Guid.Parse(toProfileId);
            if (currentProfile.FriendRequestsSent.Any(friendRequest => friendRequest.UserId == toProfileGuid) ||
                currentProfile.FriendRequestsReceived.Any(request => request.UserId == toProfileGuid) ||
                currentProfile.Friends.Any(friend => friend == toProfileGuid))
            {
                throw new GracefulDuplicateException($"user/{toProfileId}/friend");
            }

            FriendRequest friendRequestSending = new()
                {FriendRequestStatus = FriendRequestStatus.Sent, UserId = toProfileGuid};
            FriendRequest friendRequestRecv = new()
                {FriendRequestStatus = FriendRequestStatus.Sent, UserId = currentProfile.ProfileId};

            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);

            currentProfile.FriendRequestsSent.Add(friendRequestSending);
            toProfile.FriendRequestsReceived.Add(friendRequestRecv);


            await UserRepository.UpdateOneAsync(currentProfile);
            await UserRepository.UpdateOneAsync(toProfile);

            return new FriendRequestResponse()
            {
                FriendRequestStatus = FriendRequestStatus.Sent
            };
        }

        public static async Task<FriendRequestResponse> AcceptFriendRequestAsync(UserProfile currentProfile,
            string toProfileId)
        {
            Guid toProfileGuid = Guid.Parse(toProfileId);

            if (currentProfile.Friends.Any(friend => friend == toProfileGuid))
            {
                throw new GracefulDuplicateException($"user/{toProfileId}/friend");
            }

            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);

            FriendRequest requestSent = toProfile.FriendRequestsSent
                .FirstOrDefault(request => request.UserId == currentProfile.ProfileId);

            FriendRequest requestRecv = currentProfile.FriendRequestsReceived
                .FirstOrDefault(request => request.UserId == toProfileGuid);

            toProfile.FriendRequestsSent.Remove(requestSent);
            currentProfile.FriendRequestsReceived.Remove(requestRecv);

            toProfile.Friends.Add(currentProfile.ProfileId);
            currentProfile.Friends.Add(toProfileGuid);

            await UserRepository.UpdateOneAsync(toProfile);
            await UserRepository.UpdateOneAsync(currentProfile);

            return new FriendRequestResponse
            {
                FriendRequestStatus = FriendRequestStatus.Accepted
            };
        }

        public static async Task<FriendRequestResponse> DeleteFriendAsync(UserProfile currentProfile,
            string toProfileId)
        {
            Guid toProfileGuid = Guid.Parse(toProfileId);
            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);

            currentProfile.FriendRequestsReceived.RemoveAll(request => request.UserId == toProfileGuid);
            currentProfile.FriendRequestsSent.RemoveAll(request => request.UserId == toProfileGuid);
            currentProfile.Friends.RemoveAll(friendGuid => friendGuid == toProfileGuid);
            

            toProfile.FriendRequestsReceived.RemoveAll(request => request.UserId == currentProfile.ProfileId);
            toProfile.FriendRequestsSent.RemoveAll(request => request.UserId == currentProfile.ProfileId);
            toProfile.Friends.RemoveAll(friendGuid => friendGuid == currentProfile.ProfileId);

            await UserRepository.UpdateOneAsync(currentProfile);
            await UserRepository.UpdateOneAsync(toProfile);

            return new FriendRequestResponse
            {
                FriendRequestStatus = FriendRequestStatus.Ignored
            };
        }
    }
}