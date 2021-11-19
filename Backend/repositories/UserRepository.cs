using System;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend.repositories
{
    public class UserRepository : TableRepository
    {
        private static CloudTable GetUserTable()
        {
            return GetTable("users");
        }

        private static async Task<UserProfile> FindOneAsync(TableQuery<UserProfile> query)
        {
            return await FindOneAsync("users", query);
        }


        public static async Task<UserProfile> FindOneByProfileIdAsync(Guid profileId)
        {
            TableQuery<UserProfile> query = new TableQuery<UserProfile>()
                .Where(TableQuery.GenerateFilterConditionForGuid("ProfileId", QueryComparisons.Equal, profileId));

            return await FindOneAsync(query);
        }

        public static async Task<UserProfile> GetLoginProfileAsync(string email, string passwordHash)
        {
            TableQuery<UserProfile> profileQuery = new TableQuery<UserProfile>()
                .Where(TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("Password", QueryComparisons.Equal, passwordHash)
                ));

            return await FindOneAsync(profileQuery);
        }

        public static async Task<UserProfile> CreateProfileAsync(UserProfile profile)
        {
            CloudTable table = GetUserTable();

            await table.CreateIfNotExistsAsync();

            profile.PrepareForTransaction();

            TableOperation insertOperation = TableOperation.Insert(profile);

            try
            {
                TableResult result = await table.ExecuteAsync(insertOperation);

                return (UserProfile) result.Result;
            }
            catch (StorageException exception)
            {
                Console.WriteLine(exception);
                throw new DuplicateProfileException();
            }
        }
    }
}