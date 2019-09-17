using System.Net.Http;
using System.Threading.Tasks;
using IpInfoProvider.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IpInfoProvider.Services
{
    public class IPInfoProvider : IIPInfoProvider
    {
        private readonly string _url;
        private readonly string _key;
        public IPInfoProvider()
        {
            var config = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();

            _url = config["ipStackUrl"];
            _key = config["publicKey"];
        }
        public async Task<IPDetails> GetDetails(string ip)
        {
            try
            {
                string url = $"{_url}/{ip}?access_key={_key}";
                HttpClient client = new HttpClient();
                HttpResponseMessage res = await client.GetAsync(url);
                HttpContent content = res.Content;
                string data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    var result = JsonConvert.DeserializeObject<ErrorResponse>(data);
                    if (!result.Success && result.Error != null)
                    {
                        return null;
                    }
                    return JsonConvert.DeserializeObject<IPDetails>(data);

                }
                //using (HttpClient client = new HttpClient())
                //{
                //    using (HttpResponseMessage res = await client.GetAsync(url))
                //    {
                //        using (HttpContent content = res.Content)
                //        {
                //            string data = await content.ReadAsStringAsync();
                //            if (data != null)
                //            {
                //                return JsonConvert.DeserializeObject<IPDetails>(data);
                //            }
                //        }
                //    }
                //}

                return null;
            }
            catch (IPServiceNotAvailableException exc)
            {
                throw exc;
            }
        }
    }
}
