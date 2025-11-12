using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PatientUserDtoMappings
    {
        public static CreateUserRequestDto ToCreateUserRequestDtoFromPatient(this CreatePatientRequestDto patientRequest)
        {
            var tempPassword = $"Pt!{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            const int patientRoleId = (int)AppRoleEnum.Patient;

            return new CreateUserRequestDto
            {
                FirstName = patientRequest.FirstName,
                LastName = patientRequest.LastName,
                Email = patientRequest.Email,
                PhoneNumber = patientRequest.PhoneNumber,
                Address = patientRequest.Address,
                Password = tempPassword,
                RoleId = patientRoleId,
                UserName = patientRequest.Email,
                MustChangePassword = true
            };
        }

        public static UpdateUserRequestDto ToUpdateUserRequestDtoFromPatient(this CreatePatientRequestDto patientRequest)
        {

            return new UpdateUserRequestDto
            {
                UserName = patientRequest.Email,
                FirstName = patientRequest.FirstName,
                LastName = patientRequest.LastName,
                Email = patientRequest.Email,
                PhoneNumber = patientRequest.PhoneNumber,
                RoleId = (int)AppRoleEnum.Patient,
                Address = patientRequest.Address,
            };
        }
    }
}
