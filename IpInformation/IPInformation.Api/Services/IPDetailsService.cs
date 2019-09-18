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
        Task<IPDetails> FetchIpDetails(string ip);
        IEnumerable<IPDetails> GetMemoryCache();
        string GetProgessOfUpdate(string jobId);
        UpdateIpDetails InitiateUpdateInformation();
        void UpdateAllInfo(UpdateIpDetails update);
    }
    public class IPDetailsService : IIPDetailsService
    {
        private readonly SqlDbContext _ctx;
        private readonly IIPInfoProvider _ipInfoProvider;
        private readonly IMemoryCacheService _memory;
        public IPDetailsService(
            SqlDbContext ctx,
            IIPInfoProvider ipInfoProvider,
            IMemoryCacheService memory
            )
        {
            _ctx = ctx;
            _memory = memory;
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

        public async Task<IPDetails> FetchIpDetails(string ip)
        {
            IPDetails result;

            /// <summary>
            /// Returns data from memory if exists
            /// </summary>
            result = _memory.FetchFromMemory(ip);

            if (result != null)
            {
                return result;
            }

            /// <summary>
            /// Returns data from database if exists
            /// and adds it to memory
            /// </summary>
            var domainDetails = await _ctx.IPDetails.FirstOrDefaultAsync(x => x.Ip == ip)
                                                    .ConfigureAwait(false);
            var details = IPDetailsExtended.DomainToView(domainDetails);
            if (details != null)
            {
                _memory.InsertToMemory(ip, details);

                return details;
            }

            /// <summary>
            /// Gets ip info from the library
            /// If it is not null we add it to db and to memory 
            /// and finally we return the value
            /// </summary>

            result = await _ipInfoProvider.GetDetails(ip).ConfigureAwait(false);

            if (result != null)
            {
                _memory.InsertToMemory(ip, result);

                await InsertIPDetails(result, ip).ConfigureAwait(false);

                return result;
            }

            return result;
        }

        public IEnumerable<IPDetails> GetMemoryCache()
        {
            var keys = _ctx.IPDetails.Select(x => x.Ip).ToHashSet();

            var result = _memory.GetMemory(keys);

            return result;
        }

        public string GetProgessOfUpdate(string jobId)
        {
            return _memory.GetProgessOfUpdate(jobId);
        }

        public UpdateIpDetails InitiateUpdateInformation()
        {
            /// Get all the ips from DB for the update
            HashSet<string> ips = _ctx.IPDetails.Select(x => x.Ip).ToHashSet();

            /// Create the object that will be stored in memory and keep the 
            /// progress status
            UpdateIpDetails update = CreateObjectForUpdate(ips);

            /// Load the object to the memory
            bool clearOfOtherProcess = _memory.LoadIpsToMemory(update);

            if (!clearOfOtherProcess)
            {
                //  return "Another process is running! Please try again later.";
            }

            return update;
        }

        public void UpdateAllInfo(UpdateIpDetails update)
        {
            /// Start the update method and return the Id to the user
            Task.Run(() => StartUpdatingIps(update));
        }

        private async Task InsertIPDetails(IPDetails details, string ip)
        {
            IPDetailsExtended model = new IPDetailsExtended(details, ip);

            await _ctx.IPDetails.AddAsync(model).ConfigureAwait(false);

            await _ctx.SaveChangesAsync();
        }

        private async Task StartUpdatingIps(UpdateIpDetails update)
        {
            /// End the process
            if (update.Ips.Count() == 0)
            {
                _memory.RemoveItem("Update");
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

                _memory.InsertToMemory("IpUpdate", update);

                await StartUpdatingIps(update).ConfigureAwait(false);

                return;
            }

            /// Take the last items and finish
            result = await _ipInfoProvider.GetDetailsForMany(update.Ips).ConfigureAwait(false);

            UpdateMultipleIps(result, update.Ips);

            _memory.RemoveItem("Update");
        }


        /// <summary>
        /// THIS WILL NOT WORK
        /// AN IMPLEMENTATION OF BACKGROUND HOST SERVICES 
        /// SHOULD BE IMPLEMENTED OR USE OF A LIBRARY SUCH AS
        /// HANGFIRE. IN ORDER TO AVOID DELAYING THE PROJECT 
        /// I WILL SEND IT WITH THIS VARIATION
        /// </summary>
        /// <param name="details"></param>
        /// <param name="ips"></param>
        private async void UpdateMultipleIps(List<IPDetails> details, HashSet<string> ips)
        {
            if (details == null)
            {
                throw new Exception("Something went wrong");
            }
            try
            {

                var originals = await _ctx.IPDetails.Where(x => ips.Contains(x.Ip)).ToListAsync();
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
                                
                _ctx.UpdateRange(originals);

                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                var temp = ex;
            }
        }

        private UpdateIpDetails CreateObjectForUpdate(HashSet<string> ips)
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
    }
}
