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
    
    public partial class TRN_THEATRE_DOCS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRN_THEATRE_DOCS()
        {
            this.TRN_THEATRE_DOCS_LOG = new HashSet<TRN_THEATRE_DOCS_LOG>();
        }
    
        public int TH_DOC_ID { get; set; }
        public string DOC_FILEPATH { get; set; }
        public Nullable<int> T_ID { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string CREATE_USER { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string UPDATE_USER { get; set; }
        public Nullable<System.DateTime> DELETE_DATE { get; set; }
        public string DELETE_USER { get; set; }
        public bool ACTIVE { get; set; }
        public Nullable<int> DOC_ID { get; set; }
    
        public virtual MST_DOCS MST_DOCS { get; set; }
        public virtual TRN_REGISTRATION TRN_REGISTRATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRN_THEATRE_DOCS_LOG> TRN_THEATRE_DOCS_LOG { get; set; }
    }
}
