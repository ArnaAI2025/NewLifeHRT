using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Services;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class ClinicServiceController : BaseApiController<ClinicServiceController>
    {
        private readonly IClinicServiceService _clinicServiceService;
        public ClinicServiceController(IClinicServiceService clinicServiceService)
        {
            _clinicServiceService = clinicServiceService;
        }

        [HttpGet("get-all-service-by-type/{serviceTypeName}")]
        public async Task<IActionResult> GetAllServiceByType(string serviceTypeName)
        {
            var services = await _clinicServiceService.GetAllServiceByTypeAsync(serviceTypeName);
            return Ok(services);
        }

        [HttpGet("get-all-appointment-services")]
        public async Task<IActionResult> GetAllAppointmentServices()
        {
            var services = await _clinicServiceService.GetAllAppointmentServicesAsync();
            return Ok(services);
        }
    }
}
