using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class BatchMessageController : BaseApiController<ConversationController>
    {
        private readonly IBatchMessageService _messageService;
        public BatchMessageController(IBatchMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var batchMessage = await _messageService.GetAllAsync();
            
            return Ok(batchMessage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var batchMessage = await _messageService.GetByIdAsync(id);
            if (batchMessage == null) return NotFound();
            return Ok(batchMessage);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BatchMessageRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request.");

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _messageService.CreateAsync(dto, userId.Value);
            return Ok(response);
        }
        [HttpPatch("approve/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BatchMessageRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request.");

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _messageService.UpdateAsync(id, dto,(int)Status.InProgress, userId.Value);
            return Ok(response);
        }
        [HttpPatch("reject/{id}")]
        public async Task<IActionResult> RejectAsync(Guid id, [FromBody] BatchMessageRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request.");

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _messageService.UpdateAsync(id, dto, (int)Status.Rejected, userId.Value);
            return Ok(response);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _messageService.BulkDeleteAsync(request.Ids);
            return Ok(result);
        }
    }
}
