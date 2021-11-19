using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend.repositories
{
    public class BuddyRepository : TableRepository
    {
        private static CloudTable GetBuddyTable()
        {
            return TableRepository.GetTable("buddies");
        }

        public async Task<List<UserProfile>> GetFriends(Guid userProfileId)
        {
            TableQuery<Buddy> query = new TableQuery<Buddy>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForGuid("PartitionKey", QueryComparisons.Equal,
                            userProfileId),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForInt("AcceptedRequest", QueryComparisons.Equal,
                            (int) BuddyRequestStatus.Accepted)));

            List<Buddy> buddies = await FindMultipleAsync("buddies", query);

            List<UserProfile> buddyProfiles = new List<UserProfile>();

            await Task.WhenAll(
                buddies.Select(async buddy =>
                    buddyProfiles.Add(await UserRepository.FindOneByProfileIdAsync(buddy.UserTo))));
            // no idea if the above will work

            return buddyProfiles;
        }
    }
}