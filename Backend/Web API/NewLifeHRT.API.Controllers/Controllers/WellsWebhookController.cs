using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.External.Interfaces;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class WellsWebhookController : WebhookController<WellsWebhookController>
    {
        private readonly ILogger<WellsWebhookController> _logger;
        private readonly IWebhookOrderService _webhookOrderService;

        public WellsWebhookController(ILogger<WellsWebhookController> logger, IWebhookOrderService webhookOrderService)
        {
            _logger = logger;
            _webhookOrderService = webhookOrderService;
        }

        [AllowAnonymous]
        [HttpPost("webhooks/wells/order")]
        public async Task<IActionResult> ReceiveOrderWebhook([FromBody] object payload)
        {
            var raw = payload.ToString();
            _logger.LogInformation("Wells Webhook Raw Payload: {Payload}", raw);

            try
            {
                var result = await _webhookOrderService.ProcessWellsOrderWebhookAsync(raw);
                if (result.HttpStatusCode == 200)
                    return Ok(result.Message);

                return StatusCode(result.HttpStatusCode ?? 500, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed processing Wells webhook");
                return StatusCode(500, new { result = false });
            }
        }
    }
}
