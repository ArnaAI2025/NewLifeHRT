using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Api.Requests;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.DTOs;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class ProposalController : BaseApiController<ProposalController>
    {
        public readonly IProposalService _proposalService;
        public readonly IOrderService _orderService;
        public readonly IProposalDetailService _proposalDetailService;
        public ProposalController(IProposalService proposalService, IOrderService orderService, IProposalDetailService proposalDetailService)
        {
            _proposalService = proposalService;
            _orderService = orderService;
            _proposalDetailService = proposalDetailService;
        }
        [HttpGet("get-all-proposals")]
        public async Task<IActionResult> GetAllProposals([FromQuery] List<int>? ids, [FromQuery] Guid? patientId)
        {
            var proposals = await _proposalService.GetAllAsync(ids,patientId);
            return Ok(proposals);
        }
        [HttpGet("get-all-proposals-patientId/{patientId?}")]
        public async Task<IActionResult> GetAllProposalsOnPatientId(Guid? patientId)
        {
            var proposals = await _proposalService.GetAllAsync(null, patientId);
            return Ok(proposals);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var proposals = await _proposalService.GetProposalById(id);
            return Ok(proposals);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProposalRequestDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var proposal = await _proposalService.CreateProposalFromDtoAsync(dto, (int)userId);
            return Ok(proposal);
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProposalRequestDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var proposal = await _proposalService.UpdateProposalAsync(id, dto, (int)userId);
            return Ok(proposal);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkOperationRequestDto<Guid> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _proposalService.BulkDeleteProposalAsync(request.Ids);
            return Ok(result);
        }
        [HttpPatch("bulk-assign")]
        public async Task<IActionResult> BulkAssignee([FromBody] BulkOperationAssigneeRequestDto<Guid, int> request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            if (request == null || request.Ids == null || !request.Ids.Any())
                return BadRequest("Lead IDs are required.");

            if (request.Id == 0)
                return BadRequest("Assignee ID is required.");

            var result = await _proposalService.BulkAssignProposalAsync(request.Ids, request.Id, userId.Value);

            return Ok(result);
        }
        [HttpPatch("accept-proposal/{id}")]
        public async Task<IActionResult> AcceptProposal(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");                      

            var result = await _proposalService.UpdateProposalStatusAsync(id, (int)Status.Approved, null, userId.Value);
             
            return Ok(result);
        }

        [HttpPatch("accept-by-patient-proposal/{id}")]
        public async Task<IActionResult> AcceptByPatientProposal(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _proposalService.UpdateProposalStatusAsync(id, (int)Status.ApprovedByPatient, null, userId.Value);

            return Ok(result);
        }

        [HttpPatch("reject-proposal/{id}")]
        public async Task<IActionResult> RejectProposal(Guid id, [FromBody] string? description)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _proposalService.UpdateProposalStatusAsync(id, (int)Status.Rejected, description, userId.Value);

            return Ok(result);
        }

        [HttpPatch("reject-by-patient-proposal/{id}")]
        public async Task<IActionResult> RejectByPatientProposal(Guid id, [FromBody] string? description)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _proposalService.UpdateProposalStatusToRejectByPatientAsync(id, (int)Status.RejectedByPatient, description, userId.Value);

            return Ok(result);
        }

        [HttpPatch("cancel-proposal/{id}")]
        public async Task<IActionResult> CancelProposal(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var result = await _proposalService.UpdateProposalStatusAsync(id, (int)Status.Canceled, null, userId.Value);

            return Ok(result);
        }
        [HttpPost("clone/{id}")]
        public async Task<IActionResult> CloneToProposal(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var proposal = await _proposalService.CloneOrderToProposalAsync(id, (int)userId);
            return Ok(proposal);
        }
        [HttpPatch("reject-order-nomoney/{id}")]
        public async Task<IActionResult> RejectOrderNoMoney(Guid id, [FromBody] ReasonDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");

            var order = await _orderService.UpdateOrderStatusAsync(id,(int)OrderStatus.Cancel_noMoney,dto.Reason,userId.Value);

            return Ok(order);
        }

        [HttpPatch("reject-order/{id}")]
        public async Task<IActionResult> RejectOrder(Guid id, [FromBody] ReasonDto description)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var order = await _orderService.UpdateOrderStatusAsync(id, (int)OrderStatus.Cancel_rejected, description.Reason, userId.Value);

            return Ok(order);
        }

        [HttpPatch("update-proposal-details/{proposalId}")]
        public async Task<IActionResult> UpdateProposalDetails(Guid proposalId, [FromBody] BulkOperationRequestDto<ProposalDetailRequestDto> proposalDetailsRequest)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized("User not authenticated.");
            var result = await _proposalDetailService.UpdateProposalDetailsAsync(proposalDetailsRequest.Ids, proposalId, userId.Value);

            return Ok(result);
        }
    }
}
