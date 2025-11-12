using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public Guid UserServiceLinkId { get; set; }
        public string? TimezoneAbbreviation { get; set; }
    }
}
