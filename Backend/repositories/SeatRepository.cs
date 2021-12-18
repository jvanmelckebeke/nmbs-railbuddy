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

        public static async Task<List<SeatRegistration>> GetSeatRegistrationsByProfileId(Guid profileId)
        {
            QueryDefinition q = new QueryDefinition("SELECT * FROM seats s WHERE s.ProfileId = @profileId")
                .WithParameter("@profileId", profileId.ToString());

            return await FindMultipleAsync(q);
        }

        public static async Task<bool> IsProfileOnLine(string line, Guid profileId)
        {
            QueryDefinition q =
                new QueryDefinition("SELECT * FROM seats s WHERE s.VehicleName = @line AND s.ProfileId = @profileId")
                    .WithParameter("@line", line)
                    .WithParameter("@profileId", profileId.ToString());

            List<SeatRegistration> registrations = await FindMultipleAsync(q);

            return registrations.Count > 0;
        }

        public static async Task RemoveSeatRegistrationsByProfileId(Guid profileId)
        {
            Container c = GetSeatsContainer();

            List<SeatRegistration> oldRegistrations = await GetSeatRegistrationsByProfileId(profileId);

            foreach (SeatRegistration registration in oldRegistrations)
            {
                await c.DeleteItemAsync<SeatRegistration>(registration.SeatRegistrationId.ToString(),
                    new PartitionKey(registration.VehicleName));
            }
        }

        public static async Task<SeatRegistration> RegisterSeat(SeatRegistration registration)
        {
            registration.SeatRegistrationId = Guid.NewGuid();

            await RemoveSeatRegistrationsByProfileId(registration.ProfileId);

            Container c = GetSeatsContainer();
            SeatRegistration createdSeat =
                await c.CreateItemAsync(registration, new PartitionKey(registration.VehicleName));

            Debug.WriteLine(createdSeat);
            return registration;
        }
    }
}