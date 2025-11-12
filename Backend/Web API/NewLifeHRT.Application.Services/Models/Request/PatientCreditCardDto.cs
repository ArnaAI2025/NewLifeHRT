using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class PatientCreditCardDto
    {
        public Guid? PatientId { get; set; }
        public Guid? Id { get; set; }
        public int CardType { get; set; }
        public int Month { get; set; }
        public string? Year { get; set; }
        public string? CardNumber { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

    }
}
