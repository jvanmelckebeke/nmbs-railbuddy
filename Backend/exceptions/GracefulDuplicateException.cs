using System;

namespace Backend.exceptions
{
    public class GracefulDuplicateException : Exception
    {
        public string Location { get; set; }

        public GracefulDuplicateException(string location) : base("duplicate item")
        {
            Location = location;
        }
    }
}