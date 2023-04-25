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
            return Ok(await _digitalTwinsService.GetListTwinsAsync());
        }

        [HttpGet("{twinId}")]
        public async Task<IActionResult> GetTwinInfo(string twinId)
        {
            return Ok(await _digitalTwinsService.GetTempByTwinIdAsync(twinId));
        }
    }
}
