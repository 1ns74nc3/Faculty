using System.Data.Entity;
using Faculty.Logic.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Faculty.Logic.DB
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer(new ApplicationIdentityInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public IDbSet<Course> Courses { get; set; }
        public IDbSet<Journal> Journals { get; set; }
        public IDbSet<Log> Logs { get; set; }
    }
}