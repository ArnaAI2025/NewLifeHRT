using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class AppointmentServiceResponseDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string? MaxDuration { get; set; }
        public List<UserDto> Users { get; set; } = new();
    }
}
