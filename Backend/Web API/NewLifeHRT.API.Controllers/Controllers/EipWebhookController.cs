using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.External.Interfaces;
using Newtonsoft.Json;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class EipWebhookController : WebhookController<EipWebhookController>
    {
        private readonly ILogger<EipWebhookController> _logger;
        private readonly IWebhookOrderService _webhookOrderService;

        public EipWebhookController(ILogger<EipWebhookController> logger, IWebhookOrderService webhookOrderService)
        {
            _logger = logger;
            _webhookOrderService = webhookOrderService;
        }

        [AllowAnonymous]
        [HttpPost("webhooks/eip/order")]
        public async Task<IActionResult> ReceiveOrderWebhook([FromBody] object payload)
        {
            string raw = payload.ToString();
            _logger.LogInformation("EIP Webhook Raw Payload: {Payload}", raw);

            try
            {
                var result = await _webhookOrderService.ProcessEipOrderWebhookAsync(raw);
                if (result.HttpStatusCode == 200)
                    return Ok(result.Message);

                return StatusCode(result.HttpStatusCode ?? 500, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed processing EIP webhook");
                return StatusCode(500, new { result = false });
            }
        }
    }
}
