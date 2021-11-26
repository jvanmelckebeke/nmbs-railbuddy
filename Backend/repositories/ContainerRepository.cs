using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend.repositories
{
    public class ContainerRepository
    {
        protected static Container GetContainer(string tableName)
        {
            var connectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString");

            CosmosClient client = new CosmosClient(connectionString);
            Container container = client.GetContainer("railbuddy", tableName);
            Debug.WriteLine(container);
            return container;
        }

        protected static async Task<T> FindOneAsync<T>(string tableName, QueryDefinition query)
            
        {
            Container container = GetContainer(tableName);

            using FeedIterator<T> resultSet = container.GetItemQueryIterator<T>(query);
            
            while (resultSet.HasMoreResults)
            {
                FeedResponse<T> response = await resultSet.ReadNextAsync();
                return response.First();
            }

            return default;
        }

        protected static async Task<List<T>> FindMultipleAsync<T>(string tableName, QueryDefinition query)
        {
            Container container = GetContainer(tableName);

            List<T> results = new List<T>();
            
            using FeedIterator<T> resultSet = container.GetItemQueryIterator<T>(query);

            while (resultSet.HasMoreResults)
            {
                FeedResponse<T> response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}