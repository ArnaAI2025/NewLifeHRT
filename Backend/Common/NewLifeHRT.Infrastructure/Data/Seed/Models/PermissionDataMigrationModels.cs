using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Data.Seed.Models
{
    public class PermissionModel
    {
        public string Section { get; set; }
        public string Module { get; set; }
        public string EnumValue { get; set; }
        public List<OperationModel> Operations { get; set; } = new List<OperationModel>();
        public List<PageModel> Pages { get; set; } = new List<PageModel>();
    }

    public class PageModel
    {
        public string Page { get; set; }
        public string EnumValue { get; set; }
        public List<OperationModel> Operations { get; set; } = new List<OperationModel>();
    }

    public class OperationModel
    {
        public string Name { get; set; }
        public string EnumValue { get; set; }
        public int[] RoleIds { get; set; }
    }
}
