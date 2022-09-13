using System;

namespace Eindwerk.Exceptions
{
    public class NoNetworkException : Exception
    {
        public NoNetworkException() : base("Sorry, no network connection") { }
    }
}