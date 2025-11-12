using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewLifeHRT.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]  
    public class MedicalRecommendationController : BaseApiController<MedicalRecommendationController>
    {
        private readonly IMedicalRecommendationService _medicalRecommendationService;
        private readonly IMedicationTypeService _medicationTypeService;
        private readonly IFollowUpLabTestService _followUpLabTestService;

        public MedicalRecommendationController(IMedicalRecommendationService medicalRecommendationService, IMedicationTypeService medicationTypeService, IFollowUpLabTestService followUpLabTestService)
        {
            _medicalRecommendationService = medicalRecommendationService;
            _medicationTypeService = medicationTypeService;
            _followUpLabTestService = followUpLabTestService;
        }

        [HttpGet("get/{id:guid}")]
        public async Task<ActionResult<MedicalRecommendationResponseDto>> GetById(Guid id)
        {
            int? userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var dto = await _medicalRecommendationService.GetByIdAsync(id, userId.Value);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpGet("get-all-by-patientId/{patientId:guid}")]
        public async Task<ActionResult<List<MedicalRecommendationResponseDto>>> GetAllByPatientId(Guid patientId)
        {
            int? userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var dtos = await _medicalRecommendationService.GetAllByPatientIdAsync(patientId);
            return Ok(dtos);
        }
        [HttpGet("get-all-medication-type")]
        public async Task<ActionResult<List<MedicalRecommendationResponseDto>>> GetAllMedicationType()
        {
            int? userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var dtos = await _medicationTypeService.GetAllMedicationTypeAsync();
            return Ok(dtos);
        }
        [HttpGet("get-all-follow-up-tests")]
        public async Task<ActionResult<List<MedicalRecommendationResponseDto>>> GetAllFollowUpTests()
        {
            int? userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var dtos = await _followUpLabTestService.GetAllFollowUpTestAsync();
            return Ok(dtos);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MedicalRecommendationRequestDto request)
        {
            int? userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _medicalRecommendationService.CreateAsync(request, userId.Value);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] MedicalRecommendationRequestDto request)
        {
            int? userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _medicalRecommendationService.UpdateAsync(request, userId.Value);

            if (string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            int? userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var response = await _medicalRecommendationService.SoftDeleteAsync(id);
            return Ok(response);
        }
    }
}
