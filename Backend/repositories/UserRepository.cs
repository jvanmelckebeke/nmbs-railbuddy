using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.exceptions;
using Microsoft.Azure.Cosmos;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend.repositories
{
    public class UserRepository : ContainerRepository
    {
        private static Container GetUserContainer()
        {
            return GetContainer("users");
        }

        private static async Task<UserProfile> FindOneAsync(QueryDefinition query)
        {
            return await FindOneAsync<UserProfile>("users", query);
        }

        public static async Task<UserProfile> UpdateOneAsync(UserProfile profile)
        {
            Container container = GetUserContainer();

            return await container.UpsertItemAsync(profile, new PartitionKey(profile.TargetCity));
        }


        public static async Task<UserProfile> FindOneByProfileIdAsync(Guid profileId)
        {
            Debug.WriteLine($"searching for profile with profileId {profileId}");
            QueryDefinition query = new QueryDefinition("select * from users u where u.id = @profileId")
                .WithParameter("@profileId", profileId.ToString());

            return await FindOneAsync(query);
        }

        public static async Task<UserProfile> GetLoginProfileAsync(string email, string passwordHash)
        {
            QueryDefinition query =
                new QueryDefinition("select * from users u where u.email = @email and u.password = @password")
                    .WithParameter("@email", email)
                    .WithParameter("@password", passwordHash);

            return await FindOneAsync(query);
        }

        public static async Task<UserProfile> CreateProfileAsync(UserProfile profile)
        {
            Container container = GetUserContainer();

            UserProfile resp =  await container.CreateItemAsync(profile, new PartitionKey(profile.TargetCity));
            return resp;
        }

        public static async Task<FriendRequestStatus?> GetFriendRequestStatus(Guid fromProfileId, Guid toProfileId)
        {
            UserProfile fromProfile = await FindOneByProfileIdAsync(fromProfileId);
            List<FriendRequest> fromRequests = fromProfile.FriendRequestsSent;

            foreach (var request in fromRequests.Where(request => request.UserId == toProfileId))
            {
                return request.FriendRequestStatus;
            }

            return null;
        }
    }
}