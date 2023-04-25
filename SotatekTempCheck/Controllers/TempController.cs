using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SotatekTempCheck.Services;

namespace SotatekTempCheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempController : ControllerBase
    {
        private readonly IDigitalTwinsService _digitalTwinsService;
        public TempController(IDigitalTwinsService digitalTwinsService)
        {
            _digitalTwinsService = digitalTwinsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTwins()
        {
            try
            {
                return Ok(await _digitalTwinsService.GetListTwinsAsync());
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{twinId}")]
        public async Task<IActionResult> GetTwinInfo(string twinId)
        {
            try
            {
                return Ok(await _digitalTwinsService.GetTempByTwinIdAsync(twinId));
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
