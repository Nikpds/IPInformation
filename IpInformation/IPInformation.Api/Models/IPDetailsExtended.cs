using IpInfoProvider.Models;
using System;
namespace IPInformation.Api.Models
{
    public class IPDetailsExtended : IPDetails
    {
        public string Ip { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }

        public IPDetailsExtended()
        {
            Created = DateTime.UtcNow;
        }

        public IPDetailsExtended(IPDetails details, string ip)
        {
            Ip = ip;
            Created = DateTime.UtcNow;            
            Latitude = details.Latitude;
            Longitude = details.Longitude;
            Continent = details.Continent;
            Country = details.Country;
            City = details.City;
        }
    }
}
