using System;
using System.Collections.Generic;
using System.Text;

namespace VCargo_ServicePartner.Models
{
    public class ImageModel
    {
        
        public int bookingId {get; set;}
        public string  imgType { get; set; }
        public byte[] img { get; set; }
        public int createdBy { get; set; }
    }
}
