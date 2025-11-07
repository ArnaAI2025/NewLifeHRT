using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Enums
{
    public enum Status
    {
        Draft = 1,
        InProgress = 2,
        Approved = 3,
        Canceled = 4,
        Rejected = 5,
        InReview = 6,
        ApprovedByPatient = 7,
        RejectedByPatient= 8
    }
}
