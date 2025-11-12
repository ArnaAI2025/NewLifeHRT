using Microsoft.AspNetCore.Mvc;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;

namespace NewLifeHRT.API.Controllers.Controllers
{
    [ApiController]
    public class CounselorNoteController : BaseApiController<CounselorNoteController>
    {
        public readonly ICounselorNoteService _counselorNoteService;
        public CounselorNoteController(ICounselorNoteService counselorNoteService)
        {
            _counselorNoteService = counselorNoteService;
        }
        [HttpGet("get-all-active-notes-patientId/{id}")]
        public async Task<IActionResult> GetAllActivePatients(Guid id)
        {
            var councelorNotes = await _counselorNoteService.GetAllActiveNotesBasesOnIdAsync(id);
            return Ok(councelorNotes);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCounselorRequestDto request)
        {
            var userId = GetUserId();
            var response = await _counselorNoteService.CreateAsync(request, userId.Value);
            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCounselorNote(Guid id)
        {
            var userId = GetUserId();
            var councelorNotes = await _counselorNoteService.DeleteNoteAsync(id,userId.Value);
            return Ok(councelorNotes);
        }
    }
}
