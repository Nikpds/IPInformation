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

        public IPInfoController(IIPDetailsService iPService)
        {
            _iPService = iPService;
        }

        [HttpGet("{ip}")]
        public async Task<IActionResult> GetIpAddressInfo(string ip)
        {
            try
            {
                var result = await _iPService.FetchIpDetails(ip).ConfigureAwait(false);

                if (result == null)
                {
                    return BadRequest("Information for the specific ip wasn't found");
                }

                return Ok(result);

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
                var result = _iPService.GetMemoryCache();

                return Ok(result);
            }
            catch (Exception exc)
            {
                return BadRequest("An Error was occured, " + exc.Message);
            }
        }

        [HttpGet("update/all/info")]
        public IActionResult UpdateAllInfo()
        {
            try
            {
                var update = _iPService.InitiateUpdateInformation();

                _iPService.UpdateAllInfo(update);

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
                var progress = _iPService.GetProgessOfUpdate(jobId);

                return Ok(progress);

            }
            catch (Exception exc)
            {
                return BadRequest("An Error was occured, " + exc.Message);
            }
        }
    }
}