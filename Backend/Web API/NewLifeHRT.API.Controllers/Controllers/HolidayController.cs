using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class HolidayController : BaseApiController<HolidayController>
    {
        private readonly IHolidayService _holidayService;
        public HolidayController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateHoliday([FromBody] CreateHolidayRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            var holiday = await _holidayService.CreateHolidayAsync(request, userId.Value);
            return Ok(holiday);
        }

        [HttpPost("all")]
        public async Task<IActionResult> GetAllHolidays([FromBody] GetAllHolidaysRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            var holidays = await _holidayService.GetAllHolidaysAsync(request);
            return Ok(holidays);
        }
    }
}
