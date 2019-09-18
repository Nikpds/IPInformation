using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IpInfoProvider.Models;
using IpInfoProvider.Services;
using IPInformation.Api.Models;
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

                result = await _iPService.GetIpDetailsFromDb(ip);

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

        [HttpGet("update/all/info")]
        public async Task<IActionResult> UpdateAllInfo()
        {
            try
            {
                /// Get all the ips from DB for the update
                HashSet<string> ips = _iPService.GetAllIps();

                /// Create the object that will be stored in memory and keep the 
                /// progress status
                UpdateIpDetails update = _iPService.CreateObjectForUpdate(ips);

                /// Load the object to the memory
                bool clearOfOtherProcess = _memory.LoadIpsToMemory(update);

                if (!clearOfOtherProcess)
                {
                    return BadRequest("Another process is running! Please try again later.");
                }

                /// Start the update method and return the Id to the user
                await _iPService.StartUpdatingIps(update);

                return Ok(update.Id);

            }
            catch (Exception exc)
            {
                return BadRequest("An Error was occured, " + exc.Message);
            }
        }

        [HttpGet("check/update/progress/{jobId}")]
        public IActionResult GetProgessOfUpdate(string jobId)
        {
            try
            {
                var progress = _memory.GetProgessOfUpdate(jobId);

                return Ok(progress);

            }
            catch (Exception exc)
            {
                return BadRequest("An Error was occured, " + exc.Message);
            }
        }
    }
}