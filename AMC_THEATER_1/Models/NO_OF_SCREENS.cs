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
    
    public partial class NO_OF_SCREENS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NO_OF_SCREENS()
        {
            this.NO_OF_SCREENS_LOG = new HashSet<NO_OF_SCREENS_LOG>();
        }
    
        public int SCREEN_ID { get; set; }
        public int T_ID { get; set; }
        public Nullable<int> AUDIENCE_CAPACITY { get; set; }
        public string SCREEN_TYPE { get; set; }
    
        public virtual TRN_REGISTRATION TRN_REGISTRATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NO_OF_SCREENS_LOG> NO_OF_SCREENS_LOG { get; set; }
    }
}
