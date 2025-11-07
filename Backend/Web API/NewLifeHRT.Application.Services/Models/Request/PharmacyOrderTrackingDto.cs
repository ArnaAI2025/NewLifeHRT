using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class PharmacyOrderTrackingDto
    {
        public Guid? OrderId {  get; set; } 
        public int? CourierServiceId { get; set; }
        public string? TrackingNumber { get; set; }
    }
}
