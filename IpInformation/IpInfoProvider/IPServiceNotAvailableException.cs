using System;

namespace IpInfoProvider
{
    public class IPServiceNotAvailableException : Exception
    {
        public IPServiceNotAvailableException() : base("Service is not Available")
        {

        }
    }
}
