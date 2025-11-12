using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CounselorNoteMappings
    {
        public static CounselorNoteResponseDto ToCouncelorNoteResponseDto(this CounselorNote counselorNote)
        {
            return new CounselorNoteResponseDto
            {
                Id = counselorNote.Id,                
                IsActive = counselorNote.IsActive,
                IsAdminMailSent = counselorNote.IsAdminMailSent,
                IsDoctorMailSent = counselorNote.IsDoctorMailSent,
                Note = counselorNote.Note,
                Subject = counselorNote.Subject,
            };
        }
        public static List<CounselorNoteResponseDto> ToCouncelorNoteResponseDtoList(this IEnumerable<CounselorNote> counselorNotes)
        {
            return counselorNotes.Select(p => p.ToCouncelorNoteResponseDto()).ToList();
        }
    }
}
