using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PharmacyConfigurationGetAllResponseDto
    {
        public Guid Id { get; set; }
        public string PharmacyName {  get; set; }
        public string TypeName { get; set; }
        public string Status { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
