using Hangfire;
using NewLifeHRT.Infrastructure.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString(AppSettingKeys.ConnectionStringKeys.HangfireConnection)));

var app = builder.Build();
app.UseHangfireDashboard("/hangfire");

app.MapGet("/", () => "Hangfire Dashboard running...");
await app.RunAsync();
