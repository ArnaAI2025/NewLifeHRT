using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Enums
{
    public enum OrderStatus
    {
        New = 1,
        Cancel_noMoney = 2,
        Cancel_rejected = 3,
        Completed = 4,
        LifeFileProcessing = 5,
        LifeFileSuccess = 6,
        LifeFileError = 7
    }
}
