using System;
using System.Collections.Generic;
using System.Text;

namespace VCargo_ServicePartner.Models
{
 public   class Deliver
    {
        public string Id { get; set; }

        public string BookingId { get; set; }
        public string OrderStatus { get; set; }
        public string OrderRefNo { get; set; }
        public string OrderDate { get; set; }
        public string OrdeConsignee { get; set; }
        public string OrderDestination { get; set; }
    }
}
