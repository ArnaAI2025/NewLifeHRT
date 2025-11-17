using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.External.Interfaces;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class LifefileWebhookController : WebhookController<LifefileWebhookController>
    {
        private readonly ILogger<LifefileWebhookController> _logger;
        private readonly IWebhookOrderService _webhookOrderService;

        public LifefileWebhookController(ILogger<LifefileWebhookController> logger, IWebhookOrderService webhookOrderService)
        {
            _logger = logger;
            _webhookOrderService = webhookOrderService;
        }

        [AllowAnonymous]
        [HttpPost("webhooks/lifefile/order")]
        public async Task<IActionResult> ReceiveOrderWebhook([FromBody] object payload)
        {
            string raw = payload.ToString();
            _logger.LogInformation("LifeFile Webhook Raw Payload: {Payload}", raw);

            try
            {
                var result = await _webhookOrderService.ProcessLifefileOrderWebhookAsync(raw);
                if (result.HttpStatusCode == 200)
                    return Ok(result.Message);

                return StatusCode(result.HttpStatusCode ?? 500, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed processing LifeFile webhook");
                return StatusCode(500, new { result = false });
            }
        }
    }
}
