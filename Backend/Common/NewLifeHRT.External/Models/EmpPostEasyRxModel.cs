using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class EmpPostEasyRxModel
    {
        public class Request
        {
            public string ClientOrderId { get; set; }
            public string PoNumber { get; set; }
            public string DeliveryService { get; set; }
            public bool AllowOverrideDeliveryService { get; set; }
            public bool AllowOverrideEssentialCopyGuidance { get; set; }
            public string PrescriptionImageBase64 { get; set; }
            public string PrescriptionPdfBase64 { get; set; }
            public string LfPracticeId { get; set; }
            public List<NewRx> NewRxs { get; set; }
            public ReferenceFields ReferenceFields { get; set; }

        }


        public class Response
        {
            public string ClientOrderId { get; set; }
            public string EipOrderId { get; set; }
            public string Note { get; set; }
        }

        public class NewRx
        {
            public Patient Patient { get; set; }
            public Prescriber Prescriber { get; set; }
            public Medication Medication { get; set; }
        }

        public class Patient
        {
            public string ClientPatientId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public char? Gender { get; set; }
            public string DateOfBirth { get; set; }
            public Address Address { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }

        public class Prescriber
        {
            public string Npi { get; set; }
            public string StateLicenseNumber { get; set; }
            public string DeaNumber { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public Address Address { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class Address
        {
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string StateProvince { get; set; }
            public string PostalCode { get; set; }
            public string CountryCode { get; set; }
        }

        public class Medication
        {
            public string ItemDesignatorId { get; set; }
            public string ClientPrescriptionId { get; set; }
            public string EssentialCopy { get; set; }
            public string DrugDescription { get; set; }
            public int Quantity { get; set; }
            public int Refills { get; set; }
            public int DaysSupply { get; set; }
            public string WrittenDate { get; set; }
            public Diagnosis Diagnosis { get; set; }
            public string Note { get; set; }
            public string SigText { get; set; }
        }

        public class Diagnosis
        {
            public int ClinicalInformationQualifier { get; set; }
            public PrimaryDiagnosis Primary { get; set; }
        }

        public class PrimaryDiagnosis
        {
            public string Code { get; set; }
            public int Qualifier { get; set; }
            public string Description { get; set; }
            public DateOfLastOfficeVisit DateOfLastOfficeVisit { get; set; }
        }

        public class DateOfLastOfficeVisit
        {
            public string Date { get; set; }
            public string DateTime { get; set; }
        }

        public class ReferenceFields
        {
            public string Reference1 { get; set; }
            public string Reference2 { get; set; }
            public string Reference3 { get; set; }
            public string Reference4 { get; set; }
            public string Reference5 { get; set; }
        }

    }
}
