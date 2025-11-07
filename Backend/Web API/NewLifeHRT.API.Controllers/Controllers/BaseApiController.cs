using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Domain.Entities;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [Route("{tenant}/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseApiController<T> : ControllerBase
    {
        private UserManager<ApplicationUser> GetUserManager()
        {
            return HttpContext?.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        }

        protected int? GetUserId()
        {
            var userIdStr = GetUserManager().GetUserId(HttpContext.User);
            return userIdStr != null && int.TryParse(userIdStr, out var userId) ? (int?)userId : null;
        }

        protected Guid? GetLoggedInPatientId()
        {
            var userManager = GetUserManager();
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            return user?.PatientId;
        }
    }
}
