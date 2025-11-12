using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;

namespace NewLifeHRT.Application.Services.Services
{
    public class CounselorNoteService : ICounselorNoteService
    {
        private readonly ICounselorNoteRepository _councelorNoteRepository;
        public CounselorNoteService(ICounselorNoteRepository councelorNoteRepository)
        {
            _councelorNoteRepository = councelorNoteRepository;
        }
        public async Task<List<CounselorNoteResponseDto>> GetAllActiveNotesBasesOnIdAsync(Guid id)
        {
            var counselorNotes = await _councelorNoteRepository.FindAsync(a => a.IsActive == true && a.PatientId == id);
            return CounselorNoteMappings.ToCouncelorNoteResponseDtoList(counselorNotes);
        }
        public async Task<CommonOperationResponseDto<Guid?>> CreateAsync(CreateCounselorRequestDto request, int userId)
        {
            try
            {
                var counselorNote = new Domain.Entities.CounselorNote
                {
                    PatientId = request.PatientId,
                    Subject = request.Subject,
                    Note = request.Note,
                    IsAdminMailSent = request.IsAdminMailSent,
                    IsDoctorMailSent = request.IsDoctorMailSent,
                    CounselorId = request.CounselorId,
                    IsActive = true,
                    CreatedBy = userId.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                await _councelorNoteRepository.AddAsync(counselorNote);
                await _councelorNoteRepository.SaveChangesAsync();

                return new CommonOperationResponseDto<Guid?>
                {
                    Id = counselorNote.Id,
                    Message = "Counselor note created successfully"
                };
            }
            catch (Exception ex)
            {
                return new CommonOperationResponseDto<Guid?>
                {
                    Id = Guid.Empty,
                    Message = $"failed to create counselor note."
                };
            }
        }

        public async Task<CommonOperationResponseDto<Guid>> DeleteNoteAsync(Guid id, int userId)
        {
            var note = await _councelorNoteRepository.GetByIdAsync(id);

            if (note == null || !note.IsActive)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = id,
                    Message = "Counselor note not found or already deleted."
                };
            }

            note.IsActive = false;
            note.UpdatedBy = userId.ToString();
            note.UpdatedAt = DateTime.UtcNow;

            await _councelorNoteRepository.UpdateAsync(note);
            await _councelorNoteRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Id = note.Id,
                Message = "Counselor note deleted successfully."
            };
        }


    }
}
