using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PoolDetailResponseDto
    {
        public Guid PoolId { get; set; }
        public int CounselorId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Week { get; set; }
        public Guid PoolDetailId { get; set; }
        public string CounselorName { get; set; }    
    }
}
