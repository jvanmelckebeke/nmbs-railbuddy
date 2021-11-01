using System;

namespace Backend.exceptions
{
    public class WrongCredentialsException : Exception
    {
        public WrongCredentialsException() : base("user credentials are wrong")
        {
        }
    }
}