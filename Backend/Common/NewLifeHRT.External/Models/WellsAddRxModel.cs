using Microsoft.Identity.Client;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class WellsAddRxModel
    {
        public class Request
        {
            public string PrescriberNpiNumber { get; set; }
            public string PatId { get; set; }
            public string Ndc { get; set; }
            public string DrugName { get; set; }
            public int Control { get; set; }
            public string Frequency { get; set; }
            public string Route { get; set; }
            public int Dose { get; set; }
            public string DoseUnit { get; set; }
            public int Dispense { get; set; }
            public string DispenseUnit { get; set; }
            public string Directions { get; set; }
            public int Refills { get; set; }
            public string ClinicNumber { get; set; }
            public string PayType { get; set; }
            public string ShippingType { get; set; }
            public string DigitalSignature { get; set; }
            public string ShippingMethod { get; set; }
            public string LastOfficeVisitDate { get; set; }
            public string Method { get; set; }
            public WellsShippingAddress ShippingAddress { get; set; }

        }


        public class Response
        {
            public string RxNumber { get; set; }
           
        }

        public class WellsShippingAddress
        {
            public string ShippingName { get; set; }
            public string ShippingAddress1 { get; set; }
            public string ShippingCity { get; set; }
            public string ShippingState { get; set; }
            public string ShippingZip { get; set; }
        }

    }
}
