using System;

namespace Backend.exceptions
{
    public class DuplicateProfileException : Exception
    {
        public DuplicateProfileException() : base("a profile with that email address already exists")
        {
        }
    }
}