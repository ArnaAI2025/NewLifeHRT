using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ProductFullResponseDto
    {
        public Guid Id { get; set; }
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string? LabCode { get; set; }
        public bool? LabCorp { get; set; }
        public Guid? ParentId { get; set; }
        public string? ParentName { get; set; }

        public string? Description { get; set; }
        public string? Protocol { get; set; }
        public bool? Scheduled { get; set; }

        public string? WebProductName { get; set; }
        public string? WebProductDescription { get; set; }
        public bool? WebPopularMedicine { get; set; }

        public int? WebFormId { get; set; }
        public string? WebFormName { get; set; }

        public string? WebStrengths { get; set; }
        public string? WebCost { get; set; }

        public bool? EnableCalculator { get; set; }
        public bool? NewEnableCalculator { get; set; }
        public bool? PBPEnable { get; set; }

        public int? TypeId { get; set; }
        public string? TypeName { get; set; }

        public int? Category1Id { get; set; }
        public string? Category1Name { get; set; }

        public int? Category2Id { get; set; }
        public string? Category2Name { get; set; }

        public int? Category3Id { get; set; }
        public string? Category3Name { get; set; }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public DateTime ModifiedOn { get; set; }
        public bool? IsColdStorageProduct { get; set; }

    }
}
