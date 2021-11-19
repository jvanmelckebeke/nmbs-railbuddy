using System;
using Backend.repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend.Domain
{
    public class Buddy : TableEntity
    {
        public int AcceptedRequest => (int) BuddyRequestStatus;
        [IgnoreProperty] public BuddyRequestStatus BuddyRequestStatus { get; set; }

        public Guid UserFrom { get; set; }
        public Guid UserTo { get; set; }

        public Buddy(Guid userFrom, Guid userTo)
        {
            UserFrom = userFrom;
            UserTo = userTo;
            RowKey = userTo.ToString();
            PartitionKey = userFrom.ToString();
        }

        public Buddy()
        {
        }
    }
}