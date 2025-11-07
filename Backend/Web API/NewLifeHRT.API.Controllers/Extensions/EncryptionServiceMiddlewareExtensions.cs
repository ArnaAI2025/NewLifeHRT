using NewLifeHRT.API.Controllers.Middlewares;
using NewLifeHRT.Infrastructure.Middlewares;
using System.Text;

namespace NewLifeHRT.API.Controllers.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseEncryption(this IApplicationBuilder app)
            => app.UseMiddleware<EncryptionMiddleware>();

        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
