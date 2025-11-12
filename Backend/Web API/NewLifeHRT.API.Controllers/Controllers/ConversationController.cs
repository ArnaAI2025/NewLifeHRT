using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class ConversationController : BaseApiController<ConversationController>
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly ILogger<ConversationController> _logger;

        public ConversationController(IConversationService conversationService, ILogger<ConversationController> logger, IMessageService messageService)
        {
            _conversationService = conversationService;
            _logger = logger;
            _messageService = messageService;
        }

        [HttpGet("getConversationByPatientId/{patientId}")]
        public async Task<IActionResult> GetPatientConversation(Guid patientId)
        {

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _conversationService.GetConversationOrCreateByPatientOrLeadAsync(patientId, null, (int)userId);
            return Ok(response);
        }
        [HttpGet("get-unread-conversation-by-counselorId/{counselorId}")]
        public async Task<IActionResult> GetPatientConversationByCounselorId(int counselorId)
        {

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _messageService.GetUnReadMessagesByCounselorIdAsync(counselorId);
            return Ok(response);
        }
        [HttpGet("getConversationByLeadId/{leadId}")]
        public async Task<IActionResult> GetLeadConversation(Guid leadId)
        {

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _conversationService.GetConversationOrCreateByPatientOrLeadAsync(null, leadId, userId.Value);
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ConversationRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request.");

            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _conversationService.CreateConversation(dto, userId.Value);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveSms([FromForm] SmsRequestDto request)
        {
            _logger.LogInformation("Incoming SMS: {@Request}", request);
            var responseXml = await _conversationService.ProcessIncomingSmsAsync(request);
            return Content(responseXml, "application/xml");
        }
        [HttpPatch("mark-messages-as-read")]
        public async Task<IActionResult> MarkMessagesAsRead([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var response = await _messageService.UpdateIsReadAsync(request, (int)userId);
            return Ok(response);
        }

    }
}
