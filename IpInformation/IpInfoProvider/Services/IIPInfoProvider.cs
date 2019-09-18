using IpInfoProvider.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IpInfoProvider.Services
{
    public interface IIPInfoProvider
    {
        Task<IPDetails> GetDetails(string ip);
        Task<List<IPDetails>> GetDetailsForMany(HashSet<string> ips);
    }
}
