using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class GetActiveUsersRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one role must be provided.")]
        public List<int> RoleIds { get; set; } = new();
        public string? SearchTerm { get; set; } = string.Empty;
    }

}
