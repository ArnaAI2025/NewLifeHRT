using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
     public static class DropDownPatientMappings
    {
        public static CommonDropDownResponseDto<Guid> ToDropDownPatientResponseDto(this Patient patient)
        {
            return new CommonDropDownResponseDto<Guid>
            {
                Id = patient.Id,
                Value = $"{patient?.FirstName?.Trim()} {patient?.LastName?.Trim()}".Trim(),
            };
        }
        public static List<CommonDropDownResponseDto<Guid>> ToDropDownUserResponseDtoList(this IEnumerable<Patient> patients)
        {
            return patients.Select(patient => patient.ToDropDownPatientResponseDto()).ToList();
        }
    }
}
