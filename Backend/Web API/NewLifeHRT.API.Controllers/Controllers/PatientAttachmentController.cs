using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantTest.Controllers;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class PatientAttachmentController : BaseApiController<PatientController>
    {
        private readonly IPatientAttachmentService _patientAttachmentService;
        public PatientAttachmentController(IPatientAttachmentService patientAttachmentService)
        {
            _patientAttachmentService = patientAttachmentService;
        }
        [HttpPost("upload-documents")]
        public async Task<IActionResult> HandleUploadAsync([FromForm] UploadFilesRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            if (string.IsNullOrWhiteSpace(request.Id))
                return BadRequest("Id is required");

            if (request.UploadFileItemDto == null )
                return BadRequest("No files received");

            Guid? guidId = null;
            var result = new BulkOperationResponseDto();
            if (Guid.TryParse(request.Id, out var g))
            {
                guidId = g;
                result = await _patientAttachmentService.BulkUploadAttachmentAsync(request, g, userId.Value);
            }          
            return Ok(result);
        }
        [HttpGet("get-all-documents/{patientId:guid}")]
        public async Task<IActionResult> GetPatientAttachments(Guid patientId)
        {
            if (patientId == Guid.Empty)
                return BadRequest("Invalid patient ID.");

            var patientAttachments = await _patientAttachmentService.GetPatientAttachmentsAsync(patientId);
            return Ok(patientAttachments);
        }
        [HttpGet("get-document-by-id/{patientAttaxchmentId:guid}")]
        public async Task<IActionResult> GetPatientAttachment(Guid patientAttaxchmentId)
        {
            if (patientAttaxchmentId == Guid.Empty)
                return BadRequest("Invalid patient ID.");

            var patientAttachments = await _patientAttachmentService.GetPatientAttachmentAsync(patientAttaxchmentId);
            return Ok(patientAttachments);
        }
        [HttpPatch("delete-documents")]
        public async Task<IActionResult> DeleteDocuments([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var result = await _patientAttachmentService.BulkToggleDocumentStatusAsync(request.Ids,userId.Value,false);
            return Ok(result);
        }
        //[HttpPost("download-documents")]
        //public async Task<IActionResult> DownloadDocuments([FromBody] BulkOperationRequestDto<Guid> request)
        //{
        //    var userId = GetUserId();
        //    if (!userId.HasValue)
        //    {
        //        return Unauthorized("User not authenticated.");
        //    }
        //    var result = await _patientAttachmentService.GetPatientAttachmentsAsync(request.Ids);
        //    return Ok(result);
        //}

    }
}
