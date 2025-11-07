using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Response;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class TimezoneController : BaseApiController<AppointmentController>
    {
        private readonly ITimezoneService _timezoneService;

        public TimezoneController(ITimezoneService timezoneService)
        {
            _timezoneService = timezoneService;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<List<TimezoneResponseDto>>> GetAll()
        {
            var timezones = await _timezoneService.GetAllAsync();
            return Ok(timezones);
        }
    }
}
