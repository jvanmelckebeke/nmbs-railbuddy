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
using Microsoft.Extensions.Logging;
using FriendRequest = Backend.Domain.FriendRequest;

namespace Backend.services
{
    public class UserService
    {
        private ILogger _log;

        public UserService(ILogger logger)
        {
            _log = logger;
        }

        private UserProfileResponse ConvertToResponse(UserProfile source)
        {
            _log.LogDebug("converting {source} to UserProfileResponse", source);
            UserProfileResponse resp = ClassMapping.ConvertDomainDto<UserProfile, UserProfileResponse>(source);
            _log.LogDebug("convertor result: {resp}", resp);
            return resp;
        }

        public async Task<UserProfileResponse> CreateProfileAsync(UserProfile profile)
        {
            // prepare profile

            profile.ProfileId = Guid.NewGuid();

            profile.Password = Crypto.ComputeSha256(profile.Password); // encrypt the password

            profile.TargetCity ??= "UNKNOWN";
            // basically, if targetCity is null, then set it to UNKNOWN,
            // also this is probably gonna bite me in the ass later on

            _log.LogDebug("created new blank profile {prof}", profile);


            return ConvertToResponse(await UserRepository.CreateProfileAsync(profile));
        }


        public Task<UserProfileResponse> GetProfileByProfileIdAsync(string profileGuid)
        {
            _log.LogDebug("finding profile by string profileId {0}", profileGuid);
            return GetProfileByProfileIdAsync(Guid.Parse(profileGuid));
        }

        public async Task<UserProfileResponse> GetProfileByProfileIdAsync(Guid profileGuid)
        {
            _log.LogDebug("finding profile by guid profileId {0}", profileGuid);
            UserProfileResponse resp = ConvertToResponse(await UserRepository.FindOneByProfileIdAsync(profileGuid));
            _log.LogDebug("found userprofile {}", resp);
            return resp;
        }

        public async Task<List<UserProfileResponse>> GetFriendsAsync(string profileId)
        {
            _log.LogDebug("getting friends for profileId {}", profileId);
            Guid profileGuid = Guid.Parse(profileId);

            UserProfile profile = await UserRepository.FindOneByProfileIdAsync(profileGuid);
            _log.LogDebug("found a profile: {}", profile);

            List<UserProfileResponse> ret = new List<UserProfileResponse>();
            foreach (Guid friendId in profile.Friends)
            {
                ret.Add(await GetProfileByProfileIdAsync(friendId));
            }

            _log.LogDebug("found friends: {}", StringTools.FormatList(ret));

            return ret;
        }

        public FriendRequestStatus GetFriendRequestStatus(UserProfile currentProfile,
            string friendId)
        {
            EventId logId = new EventId();
            _log.LogDebug(logId, "getting friend request status for current profile {0} with friend profileId {1}", currentProfile, friendId);
            Guid friendGuid = Guid.Parse(friendId);

            if (currentProfile.Friends.Any(friend => friend == friendGuid))
            {
                _log.LogDebug(logId,"friend request status is accepted");
                return FriendRequestStatus.Accepted;
            }

            foreach (FriendRequest request in currentProfile.FriendRequestsSent.Where(request =>
                request.UserId == friendGuid))
            {
                _log.LogDebug(logId,"friend request status was sent from current profile and has status: {}", request.FriendRequestStatus);
                return request.FriendRequestStatus;
            }

            FriendRequestStatus status = currentProfile.FriendRequestsReceived
                .Where(request => request.UserId == friendGuid)
                .Select(request => request.FriendRequestStatus)
                .FirstOrDefault();
            _log.LogDebug(logId,"friend request status was sent from current profile and has status: {}", status);
            return status;
        }

        public async Task<FriendRequestResponse> CreateFriendRequestAsync(UserProfile currentProfile,
            string toProfileId)
        {
            EventId logId = new EventId();
            _log.LogDebug(logId,"creating friend request from current profile {0} to profile with id {1}", currentProfile, toProfileId);
            Guid toProfileGuid = Guid.Parse(toProfileId);
            if (currentProfile.FriendRequestsSent.Any(friendRequest => friendRequest.UserId == toProfileGuid) ||
                currentProfile.FriendRequestsReceived.Any(request => request.UserId == toProfileGuid) ||
                currentProfile.Friends.Any(friend => friend == toProfileGuid))
            {
                _log.LogWarning(logId, "there was already a friend request sent");
                throw new GracefulDuplicateException($"user/{toProfileId}/friend");
            }

            FriendRequest friendRequestSending = new FriendRequest()
                {FriendRequestStatus = FriendRequestStatus.Sent, UserId = toProfileGuid};
            _log.LogDebug(logId, "sending friend request is: {}", friendRequestSending);
            
            FriendRequest friendRequestRecv = new FriendRequest()
                {FriendRequestStatus = FriendRequestStatus.Sent, UserId = currentProfile.ProfileId};
            _log.LogDebug(logId, "receiving friend request is: {}", friendRequestSending);
            
            
            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);
            _log.LogDebug(logId, "receiving friend profile is: {}", toProfile);

            currentProfile.FriendRequestsSent.Add(friendRequestSending);
            toProfile.FriendRequestsReceived.Add(friendRequestRecv);

            _log.LogDebug(logId, "current profile sent friend requests are: {}", StringTools.FormatList(currentProfile.FriendRequestsSent));
            _log.LogDebug(logId, "receiving profile friend requests are: {}", StringTools.FormatList(toProfile.FriendRequestsReceived));

            _log.LogDebug(logId, "updating profiles");
            _log.LogDebug(logId, "updating current profile");
            await UserRepository.UpdateOneAsync(currentProfile);
            _log.LogDebug(logId, "updating to profile");
            await UserRepository.UpdateOneAsync(toProfile);
            _log.LogDebug(logId, "updated profiles");

            _log.LogDebug(logId, "creating friend request is done");
            return new FriendRequestResponse()
            {
                FriendRequestStatus = FriendRequestStatus.Sent
            };
        }

        public async Task<FriendRequestResponse> AcceptFriendRequestAsync(UserProfile currentProfile,
            string toProfileId)
        {
            EventId logId = new EventId();
            _log.LogDebug(logId, "request is to accept friend request using current profile {0} and sent friend id {1}", currentProfile, toProfileId);
            Guid toProfileGuid = Guid.Parse(toProfileId);

            if (currentProfile.Friends.Any(friend => friend == toProfileGuid))
            {
                _log.LogWarning(logId, "friend request was already accepted");
                throw new GracefulDuplicateException($"user/{toProfileId}/friend");
            }

            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);
            _log.LogDebug("friend profile is: {}", toProfile);

            FriendRequest requestSent = toProfile.FriendRequestsSent
                .FirstOrDefault(request => request.UserId == currentProfile.ProfileId);
            _log.LogDebug("sent friend request is: {}", requestSent);

            FriendRequest requestRecv = currentProfile.FriendRequestsReceived
                .FirstOrDefault(request => request.UserId == toProfileGuid);
            _log.LogDebug("sent friend request is: {}", requestSent);

            _log.LogDebug("removing friend requests from friend requests properties");
            toProfile.FriendRequestsSent.Remove(requestSent);
            currentProfile.FriendRequestsReceived.Remove(requestRecv);
            
            _log.LogDebug(logId,"adding friends to friend properties");
            toProfile.Friends.Add(currentProfile.ProfileId);
            currentProfile.Friends.Add(toProfileGuid);
            
            _log.LogDebug(logId, "updating profiles");
            _log.LogDebug(logId, "updating current profile");
            await UserRepository.UpdateOneAsync(currentProfile);
            _log.LogDebug(logId, "updating to profile");
            await UserRepository.UpdateOneAsync(toProfile);
            _log.LogDebug(logId, "updated profiles");

            _log.LogDebug(logId, "accepting friend request is done");
            return new FriendRequestResponse
            {
                FriendRequestStatus = FriendRequestStatus.Accepted
            };
        }

        public async Task<FriendRequestResponse> DeleteFriendAsync(UserProfile currentProfile,
            string toProfileId)
        {
            EventId id = new EventId();
            _log.LogDebug(id, "request is to remove friend with id {0} from current profile {1}", toProfileId, currentProfile);
            Guid toProfileGuid = Guid.Parse(toProfileId);
            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);
            _log.LogDebug(id, "the friend profile is {}", toProfile);

            _log.LogDebug(id, "removing from all requests and friends in current profile");
            currentProfile.FriendRequestsReceived.RemoveAll(request => request.UserId == toProfileGuid);
            currentProfile.FriendRequestsSent.RemoveAll(request => request.UserId == toProfileGuid);
            currentProfile.Friends.RemoveAll(friendGuid => friendGuid == toProfileGuid);
            _log.LogDebug(id, "removed from all requests and friends in current profile");


            _log.LogDebug(id, "removing from all requests and friends in friend profile");
            toProfile.FriendRequestsReceived.RemoveAll(request => request.UserId == currentProfile.ProfileId);
            toProfile.FriendRequestsSent.RemoveAll(request => request.UserId == currentProfile.ProfileId);
            toProfile.Friends.RemoveAll(friendGuid => friendGuid == currentProfile.ProfileId);
            _log.LogDebug(id, "removed from all requests and friends in friend profile");

            _log.LogDebug(id, "updating profiles");
            _log.LogDebug(id, "updating current profile");
            await UserRepository.UpdateOneAsync(currentProfile);
            _log.LogDebug(id, "updating friend profile");
            await UserRepository.UpdateOneAsync(toProfile);
            _log.LogDebug(id, "updated profiles");

            return new FriendRequestResponse
            {
                FriendRequestStatus = FriendRequestStatus.Ignored
            };
        }
    }
}