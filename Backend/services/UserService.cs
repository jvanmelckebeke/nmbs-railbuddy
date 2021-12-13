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
            _log.LogDebug("finding profile by string profileId {profileId}", profileGuid);
            return GetProfileByProfileIdAsync(Guid.Parse(profileGuid));
        }

        public async Task<UserProfileResponse> GetProfileByProfileIdAsync(Guid profileGuid)
        {
            _log.LogInformation("finding profile by guid profileId {profileId}", profileGuid);
            UserProfileResponse resp = ConvertToResponse(await UserRepository.FindOneByProfileIdAsync(profileGuid));
            _log.LogDebug("found userprofile {}", resp);
            return resp;
        }

        public async Task<List<Friend>> GetFriendsAsync(string profileId)
        {
            var id = new EventId();

            _log.LogInformation("[event: {eventId}] getting friends for profileId {profileId}", id, profileId);
            Guid profileGuid = Guid.Parse(profileId);

            UserProfile profile = await UserRepository.FindOneByProfileIdAsync(profileGuid);
            _log.LogDebug("[event: {eventId}] found a profile: {profile}", id, profile);

            _log.LogDebug("[event: {eventId}] found friends: {friends}", id, StringTools.FormatList(profile.Friends));

            return profile.Friends;
        }

        public FriendRequestStatus GetFriendRequestStatus(UserProfile currentProfile,
                                                          string friendId)
        {
            var logId = new EventId();
            _log.LogInformation(
                "[event: {eventId}] getting friend request status for current profile {currentProfile} with friend profileId {friendId}",
                logId, currentProfile, friendId);
            Guid friendGuid = Guid.Parse(friendId);

            if (currentProfile.Friends.Any(friend => friend.UserId == friendGuid))
            {
                _log.LogDebug("[event: {eventId}] friend request status is accepted", logId);
                return FriendRequestStatus.Accepted;
            }

            foreach (FriendRequest request in currentProfile.FriendRequestsSent.Where(request =>
                         request.UserId == friendGuid))
            {
                _log.LogDebug(logId,
                    "[event: {eventId}] friend request status was sent from current profile and has status: {friendStatus}",
                    logId,
                    request.FriendRequestStatus);
                return request.FriendRequestStatus;
            }

            FriendRequestStatus status = currentProfile.FriendRequestsReceived
                                                       .Where(request => request.UserId == friendGuid)
                                                       .Select(request => request.FriendRequestStatus)
                                                       .FirstOrDefault();
            _log.LogDebug(
                "[event: {eventId}] friend request status was sent from current profile and has status: {status}",
                logId, status);
            return status;
        }

        public async Task<FriendRequestResponse> CreateFriendRequestAsync(
            UserProfile currentProfile, string toProfileId)
        {
            var logId = new EventId();

            _log.LogInformation(
                "[eventId: {eventId}] creating friend request from current profile {currentProfile} to profile with id {toProfile}",
                logId,
                currentProfile, toProfileId);

            Guid toProfileGuid = Guid.Parse(toProfileId);
            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);

            if (currentProfile.FriendRequestsSent.Any(friendRequest => friendRequest.UserId == toProfileGuid) ||
                currentProfile.FriendRequestsReceived.Any(request => request.UserId == toProfileGuid) ||
                currentProfile.Friends.Any(friend => friend.UserId == toProfileGuid))
            {
                _log.LogWarning("[event: {eventId}] there was already a friend request sent", logId);
                throw new GracefulDuplicateException($"user/{toProfileId}/friend");
            }

            if (currentProfile == toProfile)
            {
                _log.LogWarning("[event: {eventId}] you cant befriend yourself", logId);
                throw new GracefulDuplicateException($"user/{toProfileId}");
            }

            var friendRequestSending = new FriendRequest()
            {
                FriendRequestStatus = FriendRequestStatus.Sent,
                UserId = toProfileGuid,
                Username = toProfile.Username,
                Email = toProfile.Email
            };

            _log.LogDebug("[event: {eventId}] sending friend request is: {receivingFriendRequest}", logId,
                friendRequestSending);

            var friendRequestRecv = new FriendRequest()
            {
                FriendRequestStatus = FriendRequestStatus.Sent,
                UserId = currentProfile.ProfileId,
                Username = currentProfile.Username,
                Email = currentProfile.Email
            };
            _log.LogDebug("[event: {eventId}] receiving friend request is: {receivingFriendRequest}", logId,
                friendRequestSending);


            _log.LogDebug(logId, "receiving friend profile is: {}", toProfile);

            currentProfile.FriendRequestsSent.Add(friendRequestSending);
            toProfile.FriendRequestsReceived.Add(friendRequestRecv);

            _log.LogDebug("[event: {eventId}] current profile sent friend requests are: {friendRequests}", logId,
                StringTools.FormatList(currentProfile.FriendRequestsSent));
            _log.LogDebug(logId, "[event: {eventId}] receiving profile friend requests are: {friendRequests}", logId,
                StringTools.FormatList(toProfile.FriendRequestsReceived));

            _log.LogDebug("[event: {eventId}] updating profiles", logId);
            _log.LogDebug("[event: {eventId}] updating current profile", logId);
            await UserRepository.UpdateOneAsync(currentProfile);
            _log.LogDebug("[event: {eventId}] updating to profile", logId);
            await UserRepository.UpdateOneAsync(toProfile);
            _log.LogDebug("[event: {eventId}] updated profiles", logId);

            _log.LogDebug("[event: {eventId}] creating friend request is done", logId);
            return new FriendRequestResponse()
            {
                FriendRequestStatus = FriendRequestStatus.Sent
            };
        }

        public async Task<FriendRequestResponse> AcceptFriendRequestAsync(UserProfile currentProfile,
                                                                          string toProfileId)
        {
            EventId logId = new EventId();
            _log.LogDebug(
                "[event: {eventId}] request is to accept friend request using current profile {0} and sent friend id {1}",
                logId,
                currentProfile, toProfileId);
            Guid toProfileGuid = Guid.Parse(toProfileId);

            if (currentProfile.Friends.Any(friend => friend.UserId == toProfileGuid))
            {
                _log.LogWarning(logId, "[event: {eventId}] friend request was already accepted", logId);
            }

            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);
            _log.LogDebug("[event: {eventId}] friend profile is: {}", logId, toProfile);

            FriendRequest requestSent = toProfile.FriendRequestsSent
                                                 .FirstOrDefault(request => request.UserId == currentProfile.ProfileId);
            _log.LogDebug("[event: {eventId}] sent friend request is: {}", logId, requestSent);

            FriendRequest requestRecv = currentProfile.FriendRequestsReceived
                                                      .FirstOrDefault(request => request.UserId == toProfileGuid);
            _log.LogDebug("[event: {eventId}] sent friend request is: {}", logId, requestSent);

            _log.LogDebug("[event: {eventId}] removing friend requests from friend requests properties", logId);
            toProfile.FriendRequestsSent.Remove(requestSent);
            currentProfile.FriendRequestsReceived.Remove(requestRecv);

            _log.LogDebug("[event: {eventId}] adding friends to friend properties", logId);

            Friend current = new Friend(currentProfile);
            Friend to = new Friend(toProfile);

            if (!toProfile.Friends.Contains(current))
            {
                toProfile.Friends.Add(current);
            }

            if (!currentProfile.Friends.Contains(to))
            {
                currentProfile.Friends.Add(to);
            }

            _log.LogDebug("[event: {eventId}] updating profiles", logId);
            _log.LogDebug("[event: {eventId}] updating current profile", logId);
            await UserRepository.UpdateOneAsync(currentProfile);
            _log.LogDebug("[event: {eventId}] updating to profile", logId);
            await UserRepository.UpdateOneAsync(toProfile);
            _log.LogDebug("[event: {eventId}] updated profiles", logId);

            _log.LogDebug("[event: {eventId}] accepting friend request is done", logId);
            return new FriendRequestResponse
            {
                FriendRequestStatus = FriendRequestStatus.Accepted
            };
        }

        public async Task<FriendRequestResponse> DeleteFriendAsync(UserProfile currentProfile,
                                                                   string toProfileId)
        {
            EventId logId = new EventId();
            _log.LogDebug("[event: {eventId}] request is to remove friend with id {0} from current profile {1}",
                logId, toProfileId,
                currentProfile);
            Guid toProfileGuid = Guid.Parse(toProfileId);
            UserProfile toProfile = await UserRepository.FindOneByProfileIdAsync(toProfileGuid);
            _log.LogDebug("[event: {eventId}] the friend profile is {}", logId, toProfile);

            _log.LogDebug("[event: {eventId}] removing from all requests and friends in current profile", logId);
            currentProfile.FriendRequestsReceived.RemoveAll(request => request.UserId == toProfileGuid);
            currentProfile.FriendRequestsSent.RemoveAll(request => request.UserId == toProfileGuid);
            currentProfile.Friends.RemoveAll(friend => friend.UserId == toProfileGuid);
            _log.LogDebug(logId, "[event: {eventId}] removed from all requests and friends in current profile", logId);


            _log.LogDebug("[event: {eventId}] removing from all requests and friends in friend profile", logId);
            toProfile.FriendRequestsReceived.RemoveAll(request => request.UserId == currentProfile.ProfileId);
            toProfile.FriendRequestsSent.RemoveAll(request => request.UserId == currentProfile.ProfileId);
            toProfile.Friends.RemoveAll(friend => friend.UserId == currentProfile.ProfileId);
            _log.LogDebug("[event: {eventId}] removed from all requests and friends in friend profile", logId);

            _log.LogDebug("[event: {eventId}] updating profiles", logId);
            _log.LogDebug("[event: {eventId}] updating current profile", logId);
            await UserRepository.UpdateOneAsync(currentProfile);
            _log.LogDebug("[event: {eventId}] updating friend profile", logId);
            await UserRepository.UpdateOneAsync(toProfile);
            _log.LogDebug("[event: {eventId}] updated profiles", logId);

            return new FriendRequestResponse
            {
                FriendRequestStatus = FriendRequestStatus.Ignored
            };
        }
    }
}