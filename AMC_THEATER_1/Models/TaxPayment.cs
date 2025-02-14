using System;
using System.Collections.Generic;

namespace AMC_THEATER_1.Models
{
    public class TaxPayment
    {
        public int TheaterID { get; set; }
        public string OwnerName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        // Add a property to hold the list of screens
        public List<ScreenViewModel> Screens { get; set; } // Assuming ScreenViewModel is defined elsewhere
    }
}