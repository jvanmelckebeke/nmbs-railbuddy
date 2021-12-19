using System;

namespace Eindwerk.Exceptions
{
    public class WrongCredentialsException : Exception
    {
        public WrongCredentialsException() : base("Email and/or password incorrect") { }
    }
}