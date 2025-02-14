using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class ScreenModel
    {
        public int S_TAX_ID { get; set; }
        public int T_ID { get; set; }
        public string ScreenType { get; set; }
        public int AudienceCapacity { get; set; }
        public int? TotalShow { get; set; }
        public int CancelShow { get; set; }
    }

}