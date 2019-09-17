using IpInfoProvider.Models;
using IPInformation.Api.DataContext;
using IPInformation.Api.Models;
using Microsoft.EntityFrameworkCore;
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
    }
    public class IPDetailsService : IIPDetailsService
    {
        private readonly SqlDbContext _ctx;
        public IPDetailsService(SqlDbContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Creates and object with total count of all ips
        /// and the ips that will be proccessed for update.
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
                Id = new Guid(),
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
    }
}
