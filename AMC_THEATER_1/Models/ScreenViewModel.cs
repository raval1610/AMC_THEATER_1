using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class ScreenViewModel
    {
  
        public int ScreenId { get; set; }
        public int AudienceCapacity { get; set; }
        public string ScreenType { get; set; }
        public string SeatCapacity { get; set; }
        public int ScreenPrice { get; set; }  // Store price from TRN_SCREEN_TAX_PRICE
        public int SEQUENTIAL_SCREEN_NO { get; set; }  // Store price from TRN_SCREEN_TAX_PRICE

        
        public int TotalShow { get; set; } // This will be populated from user input
        public int CancelShow { get; set; } // This will be populated from user input
        public decimal AmtPerScreen { get; set; } // This can be calculated based on your logic
       
    }
}