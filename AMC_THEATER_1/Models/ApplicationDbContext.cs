using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AMC_THEATER_1.Models
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext() : base("THEATER_MODULE_1Entities") { }

        public DbSet<TRN_REGISTRATION> TRN_REGISTRATION { get; set; }


        public DbSet<PAYMENT_HISTORY> PAYMENT_HISTORY { get; set; }
        public DbSet<NO_OF_SCREENS> NO_OF_SCREENS { get; set; }
        public DbSet<RECEIPT_FILTER> RECEIPT_FILTER { get; set; }

        public DbSet<T_RECEIPT> Receipts { get; set; }
        public DbSet<PENDINGDUEADMIN> PENDINGDUEADMIN { get; set; }
        public DbSet<PAYMENTLIST> PAYMENTLIST { get; set; }
        public DbSet<LOGIN_DETAILS> LOGIN_DETAILS { get; set; }
        public DbSet<DEPT_LOGIN_DETAILS> DEPT_LOGIN_DETAILS { get; set; }

        public DbSet<TRN_SCREEN_TAX_PRICE> TRN_SCREEN_TAX_PRICE { get; set; }
        public DbSet<THEATER_TAX_PAYMENT> THEATER_TAX_PAYMENT { get; set; }
        public DbSet<ActionRequest> ActionRequest { get; set; }
        public DbSet<NO_OF_SCREENS_TAX> NO_OF_SCREENS_TAX { get; set; }
        public DbSet<TRN_THEATRE_DOCS> TRN_THEATRE_DOCS { get; set; }
        public DbSet<MST_DOCS> MST_DOCS { get; set; }
        public DbSet<MonthTable> MonthTables { get; set; }



    }

}