namespace AuctionDemo.DAL.Models.ExceptionContext
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ExceptionContext : DbContext
    {
        public ExceptionContext()
            : base("name=ExceptionContext")
        {
        }

        public virtual DbSet<Exception_Log> Exception_Log { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
