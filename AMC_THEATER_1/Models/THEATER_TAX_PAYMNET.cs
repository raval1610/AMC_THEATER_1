using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class THEATER_TAX_PAYMNET
    {
        public int Id { get; set; }
        public int TheaterId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string DocumentPath { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual ICollection<ScreenTaxViewModel> Screens { get; set; } // This should be ScreenTaxViewModel
    }
}