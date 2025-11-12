using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [Route("{tenant}/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseApiController<T> : ControllerBase
    {
        protected int? GetUserId()
        {
            var userIdStr = HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdStr))
            {
                return null;
            }

            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }

        protected Guid? GetLoggedInPatientId()
        {
            var patientIdStr = HttpContext?.User?.FindFirstValue("isPatient");

            if (string.IsNullOrWhiteSpace(patientIdStr))
            {
                return null;
            }

            return Guid.TryParse(patientIdStr, out var patientId) ? patientId : null;
        }
    }
}
