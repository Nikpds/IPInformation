using IpInfoProvider.Models;
using System.Threading.Tasks;

namespace IpInfoProvider.Services
{
    public interface IIPInfoProvider
    {
        Task<IPDetails> GetDetails(string ip);
    }
}
