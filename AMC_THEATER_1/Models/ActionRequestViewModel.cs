using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class ActionRequestViewModel
    {
        public int T_ID { get; set; }
        public string T_NAME { get; set; }
        public string T_OWNER_NAME { get; set; }
        public int STATUS { get; set; }  // Add status if necessary to track approval/rejection

    }
}