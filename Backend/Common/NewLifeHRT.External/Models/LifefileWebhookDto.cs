using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class LifefileWebhookDto
    {
        public string PharmacyLocation { get; set; }
        public string FillId { get; set; }
        public string RxNumber { get; set; }
        public string ForeignRxNumber { get; set; }
        public string OrderId { get; set; }
        public string ReferenceId { get; set; }
        public string PracticeId { get; set; }
        public string ProviderId { get; set; }
        public string PatientId { get; set; }
        public string LfdrugId { get; set; }
        public string RxStatus { get; set; }
        public DateTime? RxStatusDateTime { get; set; }
        public string DeliveryService { get; set; }
        public string Service { get; set; }
        public string TrackingNumber { get; set; }
        public string ShipAddressLine1 { get; set; }
        public string ShipAddressLine2 { get; set; }
        public string ShipAddressLine3 { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public string ShipCountry { get; set; }
        public string ShipCarrier { get; set; }
        public string PatientEmail { get; set; }
    }
}
