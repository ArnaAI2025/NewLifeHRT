using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProductID { get; set; }
        public string ParentName { get; set; }
        public string Status { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
