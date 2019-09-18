using System;
using System.Collections.Generic;
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

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(url))
                    {
                        using (HttpContent content = res.Content)
                        {
                            string data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                return JsonConvert.DeserializeObject<IPDetails>(data);
                            }
                        }
                    }
                }

                return null;
            }
            catch (IPServiceNotAvailableException exc)
            {
                throw exc;
            }
        }


        /// <summary>
        /// This was intented for bulk insert in ipstack
        /// BUT MY PLAN DOES NOT SUPPORTED!!!!!!!!!!!!!!
        /// </summary>
        /// <param name="ips"></param>
        /// <returns></returns>
        public async Task<List<IPDetails>> GetDetailsWithBulk(HashSet<string> ips)
        {
            string urlIps = String.Join(',', ips);
            string url = $"{_url}/{urlIps}?access_key={_key}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(url))
                    {
                        using (HttpContent content = res.Content)
                        {
                            string data = await content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<ErrorResponse>(data);
                            if (!result.Success && result.Error != null)
                            {
                                return null;
                            }
                            return JsonConvert.DeserializeObject<List<IPDetails>>(data);
                        }
                    }
                }
            }
            catch (IPServiceNotAvailableException exc)
            {
                throw exc;
            }
        }

        public async Task<List<IPDetails>> GetDetailsForMany(HashSet<string> ips)
        {
            string urlIps = String.Join(',', ips);
            string url = $"{_url}/{urlIps}?access_key={_key}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(url))
                    {
                        using (HttpContent content = res.Content)
                        {
                            string data = await content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<ErrorResponse>(data);
                            if (!result.Success && result.Error != null)
                            {
                                return null;
                            }
                            return JsonConvert.DeserializeObject<List<IPDetails>>(data);
                        }
                    }
                }
            }
            catch (IPServiceNotAvailableException exc)
            {
                throw exc;
            }
        }
    }
}
