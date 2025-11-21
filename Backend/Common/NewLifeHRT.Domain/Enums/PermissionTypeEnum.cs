using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Enums
{
    // Will be deleted on migration squash as this type of column will be deleted
    public enum PermissionTypeEnum 
    {
        Create = 1,
        Read = 2,
        Write = 3, // Removed from DB and will be removed from enum while migration squash
        Update = 4,
        Delete = 5,
        Assign = 6,
        [Display(Name = "Not Applicable/No Access")]
        NotApplicable =7
    }
}
