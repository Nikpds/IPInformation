using IpInfoProvider.Models;
using System;
namespace IPInformation.Api.Models
{
    public class IPDetailsExtended : IPDetails
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

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
            Continent_name = details.Continent_name;
            Country_name = details.Country_name;
            City = details.City;
        }

        public static IPDetails DomainToView(IPDetailsExtended details)
        {
            if (details == null) { return null; }

            return new IPDetails()
            {
                Ip = details.Ip,
                Latitude = details.Latitude,
                Longitude = details.Longitude,
                Continent_name = details.Continent_name,
                Country_name = details.Country_name,
                City = details.City,
            };
        }
    }
}
