using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class ScreenTaxViewModel
    {
        public int ScreenId { get; set; }
        public int AudienceCapacity { get; set; }
        public string ScreenType { get; set; }
        public int TotalShow { get; set; }
        public int CancelShow { get; set; }
        public decimal AmtPerScreen { get; set; }
    }
}