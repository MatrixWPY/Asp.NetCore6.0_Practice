using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("HealthCheck")]
        public async Task<IActionResult> HealthCheck()
        {
            return Ok("I'm alive!");
        }

        [HttpGet("HealthCheckDelay")]
        public async Task<IActionResult> HealthCheckDelay()
        {
            await Task.Delay(3000); // 模擬每個請求都處理 3 秒
            return Ok("I'm alive!");
        }
    }
}
