using Microsoft.AspNetCore.SignalR;

namespace NewLifeHRT.API.Hubs
{
    public class SmsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var tenantId = httpContext.Request.Query["tenantId"];
            var patientId = httpContext.Request.Query["patientId"];

            if (!string.IsNullOrEmpty(tenantId) && !string.IsNullOrEmpty(patientId))
            {
                string tenantPatientGroup = $"{tenantId}-{patientId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, tenantPatientGroup);
            }
            await base.OnConnectedAsync();
        }
    }
}
