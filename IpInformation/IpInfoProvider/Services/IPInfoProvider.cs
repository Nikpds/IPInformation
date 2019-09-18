using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IpInfoProvider.Models;
using Newtonsoft.Json;

namespace IpInfoProvider.Services
{
    public class IPInfoProvider : IIPInfoProvider
    {
        private readonly string _url;
        private readonly string _key;
        private readonly HttpClient _client;
        public IPInfoProvider(HttpClient client, string url, string key)
        {
            _client = client;
            _url = url;
            _key = key;
        }
        public async Task<IPDetails> GetDetails(string ip)
        {
            try
            {
                string url = $"{_url}/{ip}?access_key={_key}";

                return await CallForDetails(url);
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
                var response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    if (data != null)
                    {
                        return JsonConvert.DeserializeObject<List<IPDetails>>(data);
                    }
                }

                return null;
            }
            catch (IPServiceNotAvailableException exc)
            {
                throw exc;
            }
        }

        public async Task<List<IPDetails>> GetDetailsForMany(HashSet<string> ips)
        {
            List<IPDetails> details = new List<IPDetails>();
            var ipList = ips.ToList();
            try
            {
                for (var i = 0; i < ipList.Count(); i++)
                {
                    string url = $"{_url}/{ipList[i]}?access_key={_key}";
                    var response = await CallForDetails(url);
                    if (response != null)
                    {
                        details.Add(response);
                    }
                }

                return details;
            }
            catch (IPServiceNotAvailableException exc)
            {
                throw exc;
            }
        }

        private async Task<IPDetails> CallForDetails(string url)
        {
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                if (data != null)
                {
                    return JsonConvert.DeserializeObject<IPDetails>(data);
                }
            }
            return null;
        }
    }
}
