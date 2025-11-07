using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class ShippingAddressRequestDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public bool? IsDefaultAddress { get; set; }
        public AddressDto Address { get; set; }
    }
}
