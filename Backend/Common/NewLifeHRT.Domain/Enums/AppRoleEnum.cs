using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Enums
{
    public enum AppRoleEnum
    {
        [Display(Name = "Super Admin")]
        SuperAdmin = 1,
        Admin = 2,
        Receptionist = 3,
        Nurse = 4,
        Doctor = 5,
        [Display(Name = "Sales Person")]
        SalesPerson = 6,
        Patient = 7
    }
}
