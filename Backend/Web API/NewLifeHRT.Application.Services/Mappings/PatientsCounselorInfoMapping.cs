using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PatientsCounselorInfoMapping
    {
        public static PatientCounselorInfoDto ToPatientCounselorInfoDto(this Patient patient)
        {
            return new PatientCounselorInfoDto
            {
                Id = patient.Id,
                Name = $"{patient?.FirstName?.Trim()} {patient?.LastName?.Trim()}".Trim(),
                CounselorId = patient.CounselorId
            };
        }

        public static List<PatientCounselorInfoDto> ToPatientCounselorInfoDtoList(this IEnumerable<Patient> patients)
        {
            return patients.Select(c => c.ToPatientCounselorInfoDto()).ToList();
        }
    }

}
