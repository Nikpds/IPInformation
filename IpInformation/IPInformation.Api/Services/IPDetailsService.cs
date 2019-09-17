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
        Task<IPDetails> GetIPDetails(string ip);
        IEnumerable<string> GetAllIps();
    }
    public class IPDetailsService : IIPDetailsService
    {
        private readonly SqlDbContext _ctx;
        public IPDetailsService(SqlDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<string> GetAllIps()
        {
            var ips = _ctx.IPDetails.Select(x => x.Ip).ToList();

            return ips;
        }

        public async Task<IPDetails> GetIPDetails(string ip)
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
