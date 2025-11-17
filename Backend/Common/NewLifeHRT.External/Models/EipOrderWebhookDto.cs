using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class EipOrderWebhookDto
    {
        public string SalesForceClinicAccountId { get; set; }
        public int LifeFile_Practice_ID__c { get; set; }
        public string LFProviderId { get; set; }
        public string PrescriberOrderNumber { get; set; }
        public string EipOrderId { get; set; }
        public string LFPatientId { get; set; }
        public string LFOrderId { get; set; }
        public string LFReferenceId { get; set; }
        public string ClientOrderId { get; set; }
        public string ClientPatientId { get; set; }
        public string MessageId { get; set; }
        public string CanonicalOrderId { get; set; }
        public string SalesForceOrderId { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Reference3 { get; set; }
        public string Reference4 { get; set; }
        public string Reference5 { get; set; }
        public string OrderStatus { get; set; }
        public string Error { get; set; }
        public string PrescriptionPdfBase64 { get; set; }
        public string OrderStatusLastUpdatedTime { get; set; }
        public List<EipShipmentLineDto> ShipmentLines { get; set; }
        public List<object> OrderLines { get; set; }
    }

    public class EipShipmentLineDto
    {
        public string ShipmentStatus { get; set; }
        public string ShipmentStatusLastUpdatedTime { get; set; }
        public string ShipmentTrackingNumber { get; set; }
        public string ShipmentTrackingUrl { get; set; }
        public string ShipmentProvider { get; set; }
    }
}
