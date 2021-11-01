using System;
using System.Threading.Tasks;
using Backend.Domain;
using Backend.dto;
using Backend.repositories;
using Backend.tools;

namespace Backend.services
{
    public static class UserService
    {
        private static UserProfileResponse ConvertToResponse(UserProfile source)
        {
            return ClassMapping.ConvertDomainDto<UserProfile, UserProfileResponse>(source);
        }
        
        public static async Task<UserProfileResponse> CreateProfileAsync(UserProfile profile)
        {
            // prepare profile

            profile.ProfileId = Guid.NewGuid();

            profile.Password = Crypto.ComputeSha256(profile.Password); // encrypt the password

            profile.TargetCity ??= "UNKNOWN";
            // basically, if targetCity is null, then set it to UNKNOWN, also this is probably gonna bite me in the ass later on


            return ConvertToResponse(await UserRepository.CreateProfileAsync(profile));
        }


        public static async Task<UserProfileResponse> GetProfileByProfileId(string profileId)
        {
            Guid profileGuid = Guid.Parse(profileId);

            return ConvertToResponse(await UserRepository.FindOneByProfileIdAsync(profileGuid));
        }
    }
}