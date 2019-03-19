using Faculty.Logic.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Faculty.Logic.DB
{
    public class ApplicationIdentityInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        public void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var role1 = new IdentityRole { Name = "Admin" };
            var role2 = new IdentityRole { Name = "Student" };
            var role3 = new IdentityRole { Name = "Lector" };

            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(role3);
        }

        public void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var admin1 = new ApplicationUser { UserName = "admin", Email = "admin@admin.com", Age = 21, FirstName = "Admin", LastName = "Admin", UserIsBlocked = false };
            userManager.Create(admin1, "admin1234");
            userManager.AddToRole(admin1.Id, "Admin");
            var student1 = new ApplicationUser { UserName = "student", Email = "student@student.com", Age = 24, FirstName = "Student", LastName = "Student", UserIsBlocked = false };
            userManager.Create(student1, "student1234");
            userManager.AddToRole(student1.Id, "Student");
            var lector1 = new ApplicationUser { UserName = "lector", Email = "lector@lector.com", Age = 36, FirstName = "Lector FN", LastName = "Lector LN", UserIsBlocked = false };
            userManager.Create(lector1, "lector1234");
            userManager.AddToRole(lector1.Id, "Lector");
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var courses = new List<Course>{
                new Course("Geometry", new DateTime(2019,1,31), new DateTime(2019,5,31), "Mathematics"), 
                new Course("Grammar", new DateTime(2019,2,1), new DateTime(2019,7,22), "English"),
                new Course("Space", new DateTime(2019,1,10), new DateTime(2019,6,25), "Physics")
            };

            courses.ForEach(course => context.Courses.Add(course));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            SeedRoles(roleManager);

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            SeedUsers(userManager);

            

            context.SaveChanges();
        }
    }
}
