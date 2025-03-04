using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class TaxPaymentViewModel
    {
        [Required]
        public int TheaterId { get; set; }
        public string TheaterName { get; set; }

        [Required]
        public string FromMonth { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100.")]
        public string ToMonth { get; set; }
        public string MonthYear { get; set; }


        
        public int ScreenId { get; set; }
        public int TotalShow { get; set; }
        
             public int Amount { get; set; }

        public string OwnerName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string DocumentPath { get; set; }

        public List<ScreenViewModel> Screens { get; set; } = new List<ScreenViewModel>();



    }
}