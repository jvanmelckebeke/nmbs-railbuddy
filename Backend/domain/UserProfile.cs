using System;
using Backend.tools;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend.Domain
{
    public class UserProfile : TableEntity
    {
        public Guid ProfileId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string TargetCity { get; set; }


        public UserProfile(string email, string targetCity)
        {
            RowKey = email;
            PartitionKey = targetCity;
        }

        public UserProfile()
        {
        }

        /**
         * <summary>prepares the entity for a database transaction
         * <para>checks if there is a <c>PartitionKey</c> or <c>RowKey</c> present, if not, sets them</para></summary>
         *
         * <remarks>there is probably a cleaner method for doing this</remarks>
         */
        public void PrepareForTransaction()
        {
            if (PartitionKey != null && RowKey != null) return;

            RowKey = Email;
            PartitionKey = TargetCity;
        }
    }
}