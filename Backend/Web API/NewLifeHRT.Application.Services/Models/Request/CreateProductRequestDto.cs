using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class CreateProductRequestDto
    {
        public string ProductID { get; set; }
        public string Name { get; set; }
        public bool? IsLabCorp { get; set; }
        public bool? IsColdStorageProduct { get; set; } = false;
        public string? LabCode { get; set; }
        public Guid? ParentId { get; set; }
        public int? TypeId { get; set; }
        public int? Category1Id { get; set; }
        public int? Category2Id { get; set; }
        public int? Category3Id { get; set; }
        public string? ProductDescription { get; set; }
        public string? Protocol { get; set; }
        public bool? IsScheduled { get; set; }
        public string? WebProductName { get; set; }
        public string? WebProductDescription { get; set; }
        public bool? IsWebPopularMedicine { get; set; }
        public int? WebFormId { get; set; }
        public string? WebStrength { get; set; }
        public string? WebCost { get; set; }
        public bool? IsEnabledCalculator { get; set; }
        public bool? IsNewEnabledCalculator { get; set; }
        public bool? IsPBPEnabled { get; set; }
    }
}
