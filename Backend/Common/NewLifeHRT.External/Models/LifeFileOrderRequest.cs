using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class LifeFileOrderRequest
    {
        public LifeFileMessage Message { get; set; }
        public LifeFileOrder Order { get; set; }
    }

    public class LifeFileMessage
    {
        public string Id { get; set; }
        public string? SentTime { get; set; }
    }
    public class LifeFileOrder
    {
        public LifeFilePatient Patient { get; set; }
        public LifeFilePractice Practice { get; set; }
        public LifeFileDocument? Document { get; set; }
        public LifeFilePrescriber Prescriber { get; set; }
        public List<LifeFileRx> Rxs { get; set; }
        public LifeFileShipping Shipping { get; set; }
        public LifeFileGeneral? General { get; set; }
        public LifeFileBilling? Billing { get; set; }
    }
    public class LifeFilePatient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public string Email { get; set; }
        public string? PhoneHome { get; set; }
        public string? PhoneMobile { get; set; }
        public string? PhoneWork { get; set; }
    }
    public class LifeFilePractice
    {
        public string Id { get; set; }
    }

    public class LifeFileDocument
    {
        public string? PdfBase64 { get; set; }
    }

    public class LifeFilePrescriber
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Npi { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string? Address1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? DEA { get; set; }
        public string? LicenseState { get; set; }
        public string? LicenseNumber { get; set; }
    }

    public class LifeFileRx
    {
        public string RxType { get; set; }
        public string? DrugName { get; set; }
        public string? DrugStrength { get; set; }
        public string? DrugForm { get; set; }
        public string? LfProductID { get; set; }
        public string? ForeignPmsId { get; set; }
        public int Quantity { get; set; }
        public string? QuantityUnits { get; set; }
        public string DaysSupply { get; set; }
        public string? Directions { get; set; }
        public string ScheduleCode { get; set; }
        public string ForeignRxNumber { get; set; }
        public int Refills { get; set; }
        public string? DateWritten { get; set; }
    }

    public class LifeFileShipping
    {
        public string RecipientType { get; set; }
        public string RecipientFirstName { get; set; }
        public string RecipientLastName { get; set; }
        public string? RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public string? AddressLine1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public int? Service { get; set; }
    }

    public class LifeFileGeneral
    {
        public string? Memo { get; set; }
        public string? ReferenceId { get; set; }
        public string? StatusId { get; set; }
    }

    public class LifeFileBilling
    {
        public string PayorType { get; set; }
    }

}
