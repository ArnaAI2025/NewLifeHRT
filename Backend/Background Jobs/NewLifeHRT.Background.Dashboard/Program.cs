using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

var app = builder.Build();
app.UseHangfireDashboard("/hangfire");

app.MapGet("/", () => "Hangfire Dashboard running...");
app.Run();