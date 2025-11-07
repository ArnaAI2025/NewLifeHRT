using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

public class SmsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var patientId = httpContext?.Request.Query["patientId"].ToString();
        var leadId = httpContext?.Request.Query["leadId"].ToString();

        if (!string.IsNullOrEmpty(patientId))
            await Groups.AddToGroupAsync(Context.ConnectionId, patientId);

        if (!string.IsNullOrEmpty(leadId))
            await Groups.AddToGroupAsync(Context.ConnectionId, leadId);

        await base.OnConnectedAsync();
    }
}
