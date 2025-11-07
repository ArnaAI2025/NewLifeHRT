using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class WellsAddEditPatientModel
    {
        public class Request
        {
            public int PatId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MiddleName { get; set; }
            public string DateOfBirth { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public char? Gender { get; set; }
            public string SsNumber { get; set; }
            public string DriverLicenseNbr { get; set; }
            public string DriverLicenseState { get; set; }
            public string Email { get; set; }
            public string PrescriberNpiNumber { get; set; }
            public string ExternalId { get; set; }

        }


        public class Response
        {
            public string PatientID { get; set; }
           
        }

        public class WellsErrorResponse
        {
            [JsonPropertyName("code")]
            public int Code { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("details")]
            public List<string>? Details { get; set; }
        }



    }
}
