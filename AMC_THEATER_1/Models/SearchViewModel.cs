using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class SearchViewModel
    {
        public int T_ID { get; set; }
        public string T_NAME { get; set; }
        public string T_CITY { get; set; }
        public string T_ADDRESS { get; set; }
        public string T_TENAMENT_NO { get; set; }
        public string T_OWNER_NAME { get; set; }
        public string REG_ID { get; set; }
    }
}