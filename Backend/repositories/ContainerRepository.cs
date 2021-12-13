using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;

namespace Backend.repositories
{
    public class ContainerRepository
    {
        protected static Container GetContainer(string tableName)
        {
            string connectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString");

            var client = new CosmosClient(connectionString);
            
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
                return response.IsNullOrEmpty() ? default : response.First();
            }

            Debug.WriteLine($"found none in {tableName} for query {query.QueryText}");
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