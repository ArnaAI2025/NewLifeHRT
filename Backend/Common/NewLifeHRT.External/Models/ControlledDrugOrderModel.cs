using NewLifeHRT.Infrastructure.Models.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class ControlledDrugOrderModel : TemplateBaseModel
    {
        public string PharmacyName { get; set; }
        public string OrderId { get; set; }
        public ControlledDrugOrderPatient Patient { get; set; }
        public ControlledDrugOrderPrescriber Prescriber { get; set; }
        public ControlledDrugOrderDetails Details { get; set; }
        public ControlledDrugOrderShippingDetails ShippingDetails { get; set; }
        public List<ControlledDrugOrderMedicationDetails> MedicationDetails{ get; set; } = new List<ControlledDrugOrderMedicationDetails>();

    }
    public class ControlledDrugOrderPatient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
    }

    public class ControlledDrugOrderPrescriber
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Clinic { get; set; }
        public string? Npi { get; set; }
        public string Email { get; set; }
        public string? SignatureUrl { get; set; }
    }
    public class ControlledDrugOrderDetails
    {
        public DateTime OrderDate { get; set; }
        public string DocumentType { get; set; }
        public string Interface { get; set; }
        public string Priority { get; set; }
    }

    public class ControlledDrugOrderShippingDetails
    {
        public string ShippingMethod { get; set; }
        public bool EmailNotifications { get; set; }
        public bool FreeShipping { get; set; }
        public bool SignatureRequired { get; set; }
    }
    public class ControlledDrugOrderMedicationDetails
    {
        public string DrugName { get; set; }
        public string Strength { get; set; }
        public string Form { get; set; }
        public int Quantity { get; set; }
        public string QuantityUnits { get; set; }
        public int DaysSupply { get; set; }
        public string Directions { get; set; }
    }
}
