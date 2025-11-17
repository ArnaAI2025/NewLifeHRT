using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Models.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NewLifeHRT.API.Controllers.Controllers
{

    [Route("{tenant}/api")]
    [ApiController]
    public abstract class WebhookController<T> : ControllerBase
    {
        protected WebhookController()
        {
        }
    }

}
