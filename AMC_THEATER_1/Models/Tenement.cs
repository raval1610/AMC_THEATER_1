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
    
    public partial class Tenement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tenement()
        {
            this.TenementFacilities = new HashSet<TenementFacility>();
        }
    
        public int TenementID { get; set; }
        public string TenementNumber { get; set; }
        public string Zone { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TenementFacility> TenementFacilities { get; set; }
    }
}
