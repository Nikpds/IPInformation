using IpInfoProvider.Models;
using IpInfoProvider.Services;
using IPInformation.Api.DataContext;
using IPInformation.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPInformation.Api.Services
{
    public interface IIPDetailsService
    {
        Task<IPDetails> InsertIPDetails(IPDetails details, string ip);
        Task<IPDetails> GetIpDetailsFromDb(string ip);
        HashSet<string> GetAllIps();
        UpdateIpDetails CreateObjectForUpdate(HashSet<string> ips);
        Task StartUpdatingIps(UpdateIpDetails update);
    }
    public class IPDetailsService : IIPDetailsService
    {
        private readonly SqlDbContext _ctx;
        private readonly IIPInfoProvider _ipInfoProvider;
        private readonly IMemoryCache _cache;
        public IPDetailsService(
            SqlDbContext ctx,
            IIPInfoProvider ipInfoProvider,
            IMemoryCache cache
            )
        {
            _ctx = ctx;
            _cache = cache;
            _ipInfoProvider = ipInfoProvider;
        }

        /// <summary>
        /// Creates an object with total count of all ips
        /// and the ips that will be processed for update.
        /// A counter of how many have been completed is also provided
        /// along with a Guid that a user can use to keep track 
        /// </summary>
        /// <param name="ips"></param>
        /// <returns>An object with the guid for the user to use</returns>
        public UpdateIpDetails CreateObjectForUpdate(HashSet<string> ips)
        {
            UpdateIpDetails update = new UpdateIpDetails
            {
                Ips = ips,
                Total = ips.Count(),
                Id = Guid.NewGuid().ToString(),
                Completed = 0
            };

            return update;
        }

        public HashSet<string> GetAllIps()
        {
            var ips = _ctx.IPDetails.Select(x => x.Ip).ToHashSet();

            return ips;
        }

        public async Task<IPDetails> GetIpDetailsFromDb(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new Exception("Invalid ip");
            }
            else
            {
                var details = await _ctx.IPDetails.FirstOrDefaultAsync(x => x.Ip == ip)
                                                  .ConfigureAwait(false);

                return IPDetailsExtended.DomainToView(details);
            }
        }

        public async Task<IPDetails> InsertIPDetails(IPDetails details, string ip)
        {
            if (details == null)
            {
                throw new Exception("Invalid details");
            }
            else
            {
                IPDetailsExtended model = new IPDetailsExtended(details, ip);

                await _ctx.IPDetails.AddAsync(model).ConfigureAwait(false);

                await _ctx.SaveChangesAsync();

                return details;
            }
        }

        public async Task StartUpdatingIps(UpdateIpDetails update)
        {
            /// End the process
            if (update.Ips.Count() == 0)
            {
                _cache.Remove("IpUpdate");
                return;
            }

            List<IPDetails> result;
            /// Take 10 and continue if there are plenty left
            if (update.Ips.Count() >= 10)
            {
                HashSet<string> ipToProcess = update.Ips.Take(10).ToHashSet();

                result = await _ipInfoProvider.GetDetailsForMany(ipToProcess);

                UpdateMultipleIps(result, ipToProcess);

                update.Ips = update.Ips.Skip(10).ToHashSet();

                update.Completed += 10;

                _cache.Set("IpUpdate", update);

                await StartUpdatingIps(update);
            }

            /// Take the last items and finish
            result = await _ipInfoProvider.GetDetailsForMany(update.Ips);

            UpdateMultipleIps(result, update.Ips);

            _cache.Remove("IpUpdate");
        }

        private void UpdateMultipleIps(List<IPDetails> details, HashSet<string> ips)
        {
            if(details == null)
            {
                throw new Exception("Something went wrong");
            }
            var originals = _ctx.IPDetails.Where(x => ips.Equals(x.Ip)).ToList();
            originals.ForEach(o =>
            {
                var newDetails = details.Find(f => f.Ip == o.Ip);

                o.Updated = DateTime.UtcNow;
                o.Latitude = newDetails.Latitude;
                o.Longitude = newDetails.Longitude;
                o.Continent_name = newDetails.Continent_name;
                o.Country_name = newDetails.Country_name;
                o.City = newDetails.City;
            });

            System.Threading.Thread.Sleep(3000);
            _ctx.UpdateRange(originals);

            _ctx.SaveChanges();
        }
    }
}
