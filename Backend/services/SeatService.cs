using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.dto;
using Backend.repositories;
using Microsoft.Extensions.Logging;

namespace Backend.services
{
    public class SeatService
    {
        private ILogger _logger;

        public SeatService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<SeatRegistration> RegisterSeat(SeatRegistration registration)
        {
            return await SeatRepository.RegisterSeat(registration);
        }


        public async Task<List<FriendSeatRegistration>> GetFriendsOnLine(string line, List<Friend> friends)
        {
            List<FriendSeatRegistration> profiles = new List<FriendSeatRegistration>();

            foreach (Friend friend in friends)
            {
                if (await SeatRepository.IsProfileOnLine(line, friend.UserId))
                {
                    var friendRegistration = new FriendSeatRegistration
                    {
                        Friend = friend,
                        SeatRegistration = await SeatRepository.GetSeatRegistrationByProfileId(friend.UserId)
                    };
                    profiles.Add(friendRegistration);
                }
            }

            return profiles;
        }

        public static async Task UnregisterSeat(Guid profileId)
        {
            await SeatRepository.RemoveSeatRegistrationsByProfileId(profileId);
        }

        public async Task<SeatRegistration> GetCurrentLineAsync(Guid profileId)
        {
            SeatRegistration currentSeat = await SeatRepository.GetSeatRegistrationByProfileId(profileId);
            if (currentSeat == null)
            {
                _logger.LogWarning("no current line");
            }

            return currentSeat;
        }
    }
}