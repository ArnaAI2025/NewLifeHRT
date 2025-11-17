using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Domain.Enums;

namespace MultiTenantTest.Controllers
{
    [ApiController]
    public class UserController : BaseApiController<UserController>
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-all/{roleId?}")]
        public async Task<IActionResult> GetAll(int? roleId = null)
        {
            var users = await _userService.GetAllAsync(roleId);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateUserRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var response = await _userService.CreateAsync(request, userId.Value);
            return Ok(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateUserRequestDto request)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var response = await _userService.UpdateAsync(id, request, userId.Value);
            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            var response = await _userService.PermanentDeleteAsync(id, userId.Value);
            return Ok(response);
        }

        [HttpGet("get-all-active-doctors")]
        public async Task<IActionResult> GetAllActiveDoctors()
        {
            var activeDoctors = await _userService.GetAllActiveUsersAsync((int)AppRoleEnum.Doctor);
            return Ok(activeDoctors);
        }

        [HttpGet("get-all-active-sales-person")]
        public async Task<IActionResult> GetAllActiveSalesPerson()
        {
            var activeDoctors = await _userService.GetAllActiveUsersAsync((int)AppRoleEnum.SalesPerson);
            return Ok(activeDoctors);
        }

        [HttpPatch("deactivate-bulk")]
        public Task<IActionResult> BulkDeactivate([FromBody] BulkOperationRequestDto<int> request)
            => ToggleUsersAsync(request, isActivating: true);

        [HttpPatch("activate-bulk")]
        public Task<IActionResult> BulkActivate([FromBody] BulkOperationRequestDto<int> request)
            => ToggleUsersAsync(request, isActivating: false);

        [HttpPost("get-active-users")]
        public async Task<IActionResult> GetActiveUsers([FromBody] GetActiveUsersRequestDto request)
        {
            var users = await _userService.GetActiveUsersDropDownAsync(request.RoleId, request.SearchTerm ?? string.Empty);
            return Ok(users);
        }

        [HttpGet("vacation-users")]
        public async Task<IActionResult> GetUsersOnVacation()
        {
            var users = await _userService.GetUsersOnVacationAsync();
            return Ok(users);
        }

        [HttpPost("delete-bulk")]
        public async Task<IActionResult> DeleteUsers([FromBody] BulkOperationRequestDto<int> request)
        {
            if (request.Ids == null || !request.Ids.Any())
                return BadRequest("No User IDs provided.");

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }
            await _userService.DeleteUsersAsync(request.Ids, userId.Value);
            return Ok();
        }

        private async Task<IActionResult> ToggleUsersAsync(BulkOperationRequestDto<int> request, bool isActivating)
        {
            if (request?.Ids == null || !request.Ids.Any())
            {
                return BadRequest("No User IDs provided.");
            }

            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated.");
            }

            try
            {
                var response = await _userService.BulkToggleUserStatusAsync(request.Ids, userId.Value, isActivating);
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred while processing user status.");
            }
        }
    }
}
