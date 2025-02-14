using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class Screendata
    {
        public int T_ID { get; set; }
        public int TAX_ID { get; set; }
        public int AUDIENCE_CAPACITY { get; set; }
        public string SCREEN_TYPE { get; set; }
        public int CANCEL_SHOW { get; set; }
        public int? TOTAL_SHOW { get; set; }
    }
}