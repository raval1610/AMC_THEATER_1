//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AMC_THEATER_1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NO_OF_SCREENS_TAX
    {
        public int S_TAX_ID { get; set; }
        public int T_ID { get; set; }
        public int TAX_ID { get; set; }
        public int RATE_PER_SCREEN { get; set; }
        public string SCREEN_TYPE { get; set; }
        public decimal AMT_PER_SCREEN { get; set; }
        public int CANCEL_SHOW { get; set; }
        public Nullable<int> TOTAL_SHOW { get; set; }
        public Nullable<int> ACTUAL_SHOW { get; set; }
    
        public virtual THEATER_TAX_PAYMENT THEATER_TAX_PAYMENT { get; set; }
        public virtual TRN_REGISTRATION TRN_REGISTRATION { get; set; }
    }
}
