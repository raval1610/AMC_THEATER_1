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

        [Required]
        public string Month { get; set; }
             public string TheaterName { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100.")]
        public int Year { get; set; }

        public int MonthYear { get; set; }

        // New properties for date range
        [Required(ErrorMessage = "From Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for From Date.")]
        public DateTime FromDate { get; set; }  // Represented as string for binding purposes

        [Required(ErrorMessage = "To Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for To Date.")]
        public DateTime ToDate { get; set; }  // Represented as string for binding purposes

        public int ScreenId { get; set; }
        public int TotalShow { get; set; }
        public int Amount { get; set; }

        public string OwnerName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string DocumentPath { get; set; }

        public List<ScreenViewModel> Screens { get; set; }

        // New property to store months between FromDate and ToDate
        public List<DateTime> MonthsInRange { get; set; }
    }


}