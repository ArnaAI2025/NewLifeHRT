using NewLifeHRT.External.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Interfaces
{
    public interface IWebhookOrderService
    {
        Task<WebhookProcessResult> ProcessEipOrderWebhookAsync(string rawPayload);
        Task<WebhookProcessResult> ProcessLifefileOrderWebhookAsync(string rawPayload);
        Task<WebhookProcessResult> ProcessWellsOrderWebhookAsync(string rawPayload);
    }
}
