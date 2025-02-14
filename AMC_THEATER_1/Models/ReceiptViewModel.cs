using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class ReceiptViewModel
    {
        public string ReceiptNo { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentMode { get; set; }
     //   public string T_NAME { get; set; }
        public string TheaterName { get; set; }
        public int T_ID { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string T_OWNER_NUMBER { get; set; }
    }
}