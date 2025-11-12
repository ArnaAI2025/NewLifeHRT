using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LifeFileIntegrationController : BaseApiController<LifeFileIntegrationController>
    {
        [HttpPost("receive-empower")]
        public async Task<IActionResult> ReceiveEmpower([FromBody] EmpowerWebhookRequestDto request)
        {
            return BadRequest();
        }
    }
}
