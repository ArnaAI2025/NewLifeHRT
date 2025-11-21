using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Enums
{
    public enum ActionTypeEnum
    {
        Create,
        Read,
        Update,
        ActivateDeactivate,
        Delete,
        AcceptReject,
        Cancel,
        Clone,
        ApproveReject,
        MarkSelectedAsSeen,
        MarkAsComplete,
        Retry
    }
}
