using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class ReceiptFilterViewModel
    {
        public int? TheaterId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string StatusFilter { get; set; }
        public string PaymentModeFilter { get; set; }
        // ✅ Store raw DateTime (nullable)
        public DateTime? RCPT_GEN_DATE { get; set; }

        // ✅ Computed property for displaying the formatted date
        public string RCPT_GEN_DATE_FORMATTED
        {
            get
            {
                return RCPT_GEN_DATE.HasValue ? RCPT_GEN_DATE.Value.ToString("dd-MMM-yyyy") : "";
            }
        }
        
        public int? MonthFilter { get; set; }
        public int? YearFilter { get; set; }

        // Properties for displaying results
        public int RCPT_NO { get; set; }
       // public DateTime RCPT_GEN_DATE { get; set; }
        public int T_ID { get; set; }
        public string T_NAME { get; set; }
        public string PAY_MODE { get; set; }
        public string STATUS { get; set; }

        
    }
}