using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Backend.Domain;
using Microsoft.Azure.Cosmos;

namespace Backend.repositories
{
    public class SeatRepository : ContainerRepository
    {
        private static Container GetSeatsContainer()
        {
            return GetContainer("seats");
        }

        private static async Task<SeatRegistration> FindOneAsync(QueryDefinition query)
        {
            return await FindOneAsync<SeatRegistration>("seats", query);
        }
        
        private static async Task<List<SeatRegistration>> FindMultipleAsync(QueryDefinition query)
        {
            return await FindMultipleAsync<SeatRegistration>("seats", query);
        }

        public static async Task<SeatRegistration> GetSeatRegistrationByProfileId(Guid profileId)
        {
            QueryDefinition q = new QueryDefinition("SELECT * FROM seats s WHERE s.ProfileId = @profileId")
                .WithParameter("@profileId", profileId.ToString());

            return await FindOneAsync(q);
        }


        public static async Task<bool> IsProfileOnLine(string line, Guid profileId)
        {
            QueryDefinition q =
                new QueryDefinition("SELECT * FROM seats s WHERE s.TrainNumber = @line AND s.ProfileId = @profileId")
                    .WithParameter("@line", line)
                    .WithParameter("@profileId", profileId.ToString());

            List<SeatRegistration> registrations = await FindMultipleAsync(q);

            return registrations.Count > 0;
        }

        public static async Task RemoveSeatRegistrationByProfileId(Guid profileId)
        {
            Container c = GetSeatsContainer();

            SeatRegistration oldRegistration = await GetSeatRegistrationByProfileId(profileId);

            if (oldRegistration == null)
            {
                return;
            }

            await c.DeleteItemAsync<SeatRegistration>(oldRegistration.SeatRegistrationId.ToString(), new PartitionKey(oldRegistration.TrainNumber));
        }

        public static async Task<SeatRegistration> RegisterSeat(SeatRegistration registration)
        {
            registration.SeatRegistrationId = Guid.NewGuid();

            await RemoveSeatRegistrationByProfileId(registration.ProfileId);

            Container c = GetSeatsContainer();
            SeatRegistration createdSeat = await c.CreateItemAsync(registration, new PartitionKey(registration.TrainNumber));

            Debug.WriteLine(createdSeat);
            return registration;
        }
    }
}