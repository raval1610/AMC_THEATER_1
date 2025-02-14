using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class PaymentHistoryViewModel
    {
        public int PAYMENT_ID { get; set; }
        public int? T_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public string STATUS { get; set; }
        public string PAYMENT_DATE { get; set; } // Formatted date as string
    }
}