using IpInfoProvider.Models;
using IPInformation.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace IPInformation.Api.Services
{
    public interface IMemoryCacheService
    {
        IPDetails FetchFromMemory(string ip);
        void InsertToMemory(string ip, object key);
        IEnumerable<IPDetails> GetMemory(IEnumerable<string> keys);
        bool LoadIpsToMemory(UpdateIpDetails update);
        string GetProgessOfUpdate(string id);

        void RemoveItem(string key);
    }
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IPDetails FetchFromMemory(string ip)
        {
            if (_cache.TryGetValue(ip, out IPDetails details))
            {
                return details;
            }

            return null;
        }

        public IEnumerable<IPDetails> GetMemory(IEnumerable<string> keys)
        {

            List<IPDetails> memoryItems = new List<IPDetails>();
            foreach (var key in keys)
            {
                if (_cache.TryGetValue(key, out IPDetails details))
                {
                    memoryItems.Add(details);
                }
            }
            return memoryItems;
        }

        public void InsertToMemory(string ip, object key)
        {
            _cache.Set(ip, key);
        }

        /// <summary>
        /// This method will load all ips to memory
        /// in order to update the ipdetails and keep track
        /// of the progress. If its not null then another process is 
        /// on and will be skiped
        /// </summary>
        /// <param name="ips"></param>
        public bool LoadIpsToMemory(UpdateIpDetails update)
        {
            if (_cache.Get("IpUpdate") == null)
            {
                _cache.Set("IpUpdate", update);

                return true;
            }

            return false;

        }

        public string GetProgessOfUpdate(string id)
        {
            if (_cache.TryGetValue("IpUpdate", out UpdateIpDetails update))
            {   /// An update may be on progress but wrong id was given
                if (update.Id == id)
                {
                    return $"Currently completed: {update.Completed}/{update.Total}";
                }
            }

            return "No Such process";
        }

        public void RemoveItem(string key)
        {
            _cache.Remove(key);
        }
    }
}
