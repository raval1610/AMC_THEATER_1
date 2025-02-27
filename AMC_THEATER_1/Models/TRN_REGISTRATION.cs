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
    
    public partial class TRN_REGISTRATION
    {
        internal readonly string SCREEN_TYPE;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRN_REGISTRATION()
        {
            this.NO_OF_SCREENS = new HashSet<NO_OF_SCREENS>();
            this.NO_OF_SCREENS_LOG = new HashSet<NO_OF_SCREENS_LOG>();
            this.NO_OF_SCREENS_TAX = new HashSet<NO_OF_SCREENS_TAX>();
            this.PAYMENT_HISTORY = new HashSet<PAYMENT_HISTORY>();
            this.PAYMENTLISTs = new HashSet<PAYMENTLIST>();
            this.PENDINGDUEADMINs = new HashSet<PENDINGDUEADMIN>();
            this.RECEIPT_FILTER = new HashSet<RECEIPT_FILTER>();
            this.T_RECEIPT = new HashSet<T_RECEIPT>();
            this.THEATER_TAX_PAYMENT = new HashSet<THEATER_TAX_PAYMENT>();
            this.MST_STATUS = new HashSet<MST_STATUS>();
            this.TRN_THEATRE_DOCS = new HashSet<TRN_THEATRE_DOCS>();
            this.TRN_REGISTRATION_LOG = new HashSet<TRN_REGISTRATION_LOG>();
            this.TRN_THEATRE_DOCS_LOG = new HashSet<TRN_THEATRE_DOCS_LOG>();
        }
    
        public int T_ID { get; set; }
        public string T_NAME { get; set; }
        public string T_ADDRESS { get; set; }
        public Nullable<bool> T_ACTIVE { get; set; }
        public string T_OWNER_NAME { get; set; }
        public Nullable<long> T_OWNER_NUMBER { get; set; }
        public string T_OWNER_EMAIL { get; set; }
        public Nullable<System.DateTime> T_COMMENCEMENT_DATE { get; set; }
        public string T_CITY { get; set; }
        public string T_ZONE { get; set; }
        public string T_WARD { get; set; }
        public string T_TENAMENT_NO { get; set; }
        public string T_PEC_NO { get; set; }
        public string T_PRC_NO { get; set; }
        public Nullable<bool> T_TAX_PAYING_OFFLINE { get; set; }
        public Nullable<int> OFFLINE_TAX_PAYMENT { get; set; }
        public Nullable<System.DateTime> OFFLINE_TAX_PAID_DATE { get; set; }
        public Nullable<System.DateTime> OFFLINE_DUE_DATE { get; set; }
        public string CREATE_USER { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string UPDATE_USER { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string DELETE_USER { get; set; }
        public Nullable<System.DateTime> DELETE_DATE { get; set; }
        public string STATUS { get; set; }
        public string REJECT_REASON { get; set; }
        public string REG_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NO_OF_SCREENS> NO_OF_SCREENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NO_OF_SCREENS_LOG> NO_OF_SCREENS_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NO_OF_SCREENS_TAX> NO_OF_SCREENS_TAX { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAYMENT_HISTORY> PAYMENT_HISTORY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAYMENTLIST> PAYMENTLISTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PENDINGDUEADMIN> PENDINGDUEADMINs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECEIPT_FILTER> RECEIPT_FILTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEIPT> T_RECEIPT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<THEATER_TAX_PAYMENT> THEATER_TAX_PAYMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MST_STATUS> MST_STATUS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRN_THEATRE_DOCS> TRN_THEATRE_DOCS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRN_REGISTRATION_LOG> TRN_REGISTRATION_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRN_THEATRE_DOCS_LOG> TRN_THEATRE_DOCS_LOG { get; set; }
        public bool IsEditMode { get; internal set; }
    }
}
