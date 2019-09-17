using System;
using System.Threading.Tasks;
using IpInfoProvider.Models;
using IpInfoProvider.Services;
using IPInformation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IPInformation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPInfoController : ControllerBase
    {
        private readonly IIPDetailsService _iPService;
        private readonly IMemoryCacheService _memory;
        private readonly IIPInfoProvider _ipInfoProvider;

        public IPInfoController(
            IIPDetailsService iPService,
            IMemoryCacheService memory,
            IIPInfoProvider ipInfoProvider
            )
        {
            _iPService = iPService;
            _memory = memory;
            _ipInfoProvider = ipInfoProvider;
        }

        [HttpGet("{ip}")]
        public async Task<IActionResult> GetIpAddressInfo(string ip)
        {
            IPDetails result;
            try
            {
                /// <summary>
                /// Returns data from memory if exists
                /// </summary>
                result = _memory.FetchFromMemory(ip);

                if (result != null)
                {
                    return Ok(result);
                }

                /// <summary>
                /// Returns data from database if exists
                /// and adds it to memory
                /// </summary>

                result = await _iPService.GetIPDetails(ip);

                if (result != null)
                {
                    _memory.InsertToMemory(result, ip);

                    return Ok(result);
                }

                /// <summary>
                /// Gets ip info from the library
                /// If it is not null we add it to db and to memory 
                /// and finally we return the value
                /// </summary>

                result = await _ipInfoProvider.GetDetails(ip);

                if (result != null)
                {
                    _memory.InsertToMemory(result, ip);

                    await _iPService.InsertIPDetails(result, ip);

                    return Ok(result);
                }

                return BadRequest("Information for the specific ip wasn't found");

            }
            catch (Exception exc)
            {
                return BadRequest("An Error was occured, " + exc.Message);
            }
        }


        /// <summary>
        /// Test Call to check memory Items
        /// </summary>
        /// <returns> A list of all IPDetails stored in memory </returns>
        [HttpGet("memory")]
        public IActionResult GetMemoryCache()
        {
            try
            {
                var keys = _iPService.GetAllIps();

                var result = _memory.GetMemory(keys);

                return Ok(result);

            }
            catch (Exception exc)
            {
                return BadRequest("An Error was occured, " + exc.Message);
            }
        }
    }
}