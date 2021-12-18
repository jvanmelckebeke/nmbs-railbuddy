using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.repositories;
using Microsoft.Extensions.Logging;

namespace Backend.services
{
    public class SeatService
    {
        public static async Task<SeatRegistration> RegisterSeat(SeatRegistration registration)
        {
            return await SeatRepository.RegisterSeat(registration);
        }


        public static async Task<List<UserProfile>> GetFriendsOnLine(string line, List<Friend> friends)
        {
            List<UserProfile> profiles = new List<UserProfile>();

            foreach (Friend friend in friends)
            {
                if (await SeatRepository.IsProfileOnLine(line, friend.UserId))
                {
                    profiles.Add(await UserRepository.FindOneByProfileIdAsync(friend.UserId));
                }
            }

            return profiles;
        }

        public static async Task UnregisterSeat(Guid profileId)
        {
            await SeatRepository.RemoveSeatRegistrationByProfileId(profileId);
        }
    }
}