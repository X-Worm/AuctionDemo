namespace AuctionDemo.DAL.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AuctionContext : DbContext
    {
        public AuctionContext()
            : base("name=AuctionContext")
        {
        }

        public virtual DbSet<Bid> Bid { get; set; }
        public virtual DbSet<Lot> Lot { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<User_Configuration> User_Configuration { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bid>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<Lot>()
                .HasMany(e => e.Bid)
                .WithRequired(e => e.Lot)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Bid)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Lot)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.User_Configuration)
                .WithRequired(e => e.User);
        }
    }
}
