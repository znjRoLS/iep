using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace iep_newer.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Tokens { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //Database.SetInitializer<ApplicationDbContext>(new SchoolDBInitializer<ApplicationDbContext>());

        }

        //protected override void OnModelCreating(DbModelBuilder builder)
        //{
        //    builder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
        //    builder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
        //    builder.Entity<IdentityUserRole>().HasKey(r => new
        //    {
        //        r.RoleId,
        //        r.UserId
        //    });
        //}

        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        //public class SchoolDBInitializer<T> : CreateDatabaseIfNotExists<ApplicationDbContext>
        //{
        //    protected override void Seed(ApplicationDbContext context)
        //    {
        //        base.Seed(context);


        //        var roleStore = new RoleStore<IdentityRole>(context);
        //        var roleManager = new RoleManager<IdentityRole>(roleStore);

        //        roleManager.CreateAsync(new IdentityRole { Name = "Administrator" });

        //        var userStore = new UserStore<ApplicationUser>(context);
        //        var userManager = new UserManager<ApplicationUser>(userStore);

        //        var user = new ApplicationUser { UserName = "admin" };
        //        userManager.CreateAsync(user);
        //        userManager.AddToRoleAsync(user.Id, "Administrator");
        //    }
        //}
    }

    
}