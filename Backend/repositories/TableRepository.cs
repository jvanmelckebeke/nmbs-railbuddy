using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend.repositories
{
    public class TableRepository
    {
        protected static CloudTable GetTable(string tableName)
        {
            var connectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            return cloudTableClient.GetTableReference(tableName);
        }

        protected static async Task<T> FindOneAsync<T>(string tableName, TableQuery<T> query)
            where T : TableEntity, new()
        {
            CloudTable table = GetTable(tableName);

            TableQuerySegment<T> queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            return queryResult.Results.Count > 0 ? queryResult.Results[0] : null;
        }

        protected static async Task<List<T>> FindMultipleAsync<T>(string tableName, TableQuery<T> query)
            where T : TableEntity, new()
        {
            CloudTable table = GetTable(tableName);

            TableQuerySegment<T> queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            return queryResult.Results.Count > 0 ? queryResult.Results.ToList() : null;
        }
    }
}