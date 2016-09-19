namespace iep_newer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Models;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}
