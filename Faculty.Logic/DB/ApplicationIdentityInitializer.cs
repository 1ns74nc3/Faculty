using Faculty.Logic.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    public class ApplicationIdentityInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
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
            var admin1 = new ApplicationUser {
                UserName = "admin",
                Email = "admin@admin.com",
                Age = 21,
                FirstName = "Admin",
                LastName = "Admin",
                UserIsBlocked = false
            };
            userManager.Create(admin1, "admin1234");
            userManager.AddToRole(admin1.Id, "Admin");
            //students
            var student1 = new ApplicationUser {
                UserName = "student1",
                Email = "student@student.com",
                Age = 24,
                FirstName = "Alex",
                LastName = "Voicic",
                UserIsBlocked = false
            };
            userManager.Create(student1, "student1234");
            userManager.AddToRole(student1.Id, "Student");

            var student2 = new ApplicationUser {
                UserName = "student2",
                Email = "student@student.com",
                Age = 19,
                FirstName = "Deril",
                LastName = "Calvis",
                UserIsBlocked = false
            };
            userManager.Create(student2, "student1234");
            userManager.AddToRole(student2.Id, "Student");

            var student3 = new ApplicationUser {
                UserName = "student3",
                Email = "student@student.com",
                Age = 29,
                FirstName = "Julien",
                LastName = "Mueler",
                UserIsBlocked = false
            };
            userManager.Create(student3, "student1234");
            userManager.AddToRole(student3.Id, "Student");

            var student4 = new ApplicationUser {
                UserName = "student4",
                Email = "student@student.com",
                Age = 40,
                FirstName = "Harrison",
                LastName = "Ford",
                UserIsBlocked = false
            };
            userManager.Create(student4, "student1234");
            userManager.AddToRole(student4.Id, "Student");

            var student5 = new ApplicationUser {
                UserName = "student5",
                Email = "student@student.com",
                Age = 35,
                FirstName = "Bruce",
                LastName = "Lee",
                UserIsBlocked = false
            };
            userManager.Create(student5, "student1234");
            userManager.AddToRole(student5.Id, "Student");

            var student6 = new ApplicationUser {
                UserName = "student6",
                Email = "student@student.com",
                Age = 28,
                FirstName = "Kevin",
                LastName = "Standford",
                UserIsBlocked = false
            };
            userManager.Create(student6, "student1234");
            userManager.AddToRole(student6.Id, "Student");

            var student7 = new ApplicationUser {
                UserName = "student7",
                Email = "student@student.com",
                Age = 21,
                FirstName = "Jack",
                LastName = "Meadows",
                UserIsBlocked = false
            };
            userManager.Create(student7, "student1234");
            userManager.AddToRole(student7.Id, "Student");

            var student8 = new ApplicationUser {
                UserName = "student8",
                Email = "student@student.com",
                Age = 25,
                FirstName = "Garry",
                LastName = "Julez",
                UserIsBlocked = false
            };
            userManager.Create(student8, "student1234");
            userManager.AddToRole(student8.Id, "Student");

            var student9 = new ApplicationUser
            {
                UserName = "student9",
                Email = "student@student.com",
                Age = 18,
                FirstName = "April",
                LastName = "Pizzanio",
                UserIsBlocked = false
            };
            userManager.Create(student9, "student1234");
            userManager.AddToRole(student9.Id, "Student");

            var student10 = new ApplicationUser
            {
                UserName = "student10",
                Email = "student@student.com",
                Age = 21,
                FirstName = "Kate",
                LastName = "Middleton",
                UserIsBlocked = false
            };
            userManager.Create(student10, "student1234");
            userManager.AddToRole(student10.Id, "Student");

            //lectors
            var lector1 = new ApplicationUser {
                UserName = "lector1",
                Email = "lector1@lector.com",
                Age = 36,
                FirstName = "Greatest",
                LastName = "Lector",
                UserIsBlocked = false
            };
            userManager.Create(lector1, "lector1234");
            userManager.AddToRole(lector1.Id, "Lector");

            var lector2 = new ApplicationUser {
                UserName = "lector2",
                Email = "lector2@lector.com",
                Age = 36,
                FirstName = "Debra",
                LastName = "Hannigan",
                UserIsBlocked = false
            };
            userManager.Create(lector2, "lector1234");
            userManager.AddToRole(lector2.Id, "Lector");

            var lector3 = new ApplicationUser {
                UserName = "lector3",
                Email = "lector3@lector.com",
                Age = 36,
                FirstName = "Travis",
                LastName = "Joulz",
                UserIsBlocked = false
            };
            userManager.Create(lector3, "lector1234");
            userManager.AddToRole(lector3.Id, "Lector");

            var lector4 = new ApplicationUser {
                UserName = "lector4",
                Email = "lector4@lector.com",
                Age = 36,
                FirstName = "Mark",
                LastName = "Cristian",
                UserIsBlocked = false
            };
            userManager.Create(lector4, "lector1234");
            userManager.AddToRole(lector4.Id, "Lector");

            var lector5 = new ApplicationUser {
                UserName = "lector5",
                Email = "lector5@lector.com",
                Age = 36, FirstName = "Duck",
                LastName = "Tales",
                UserIsBlocked = false
            };
            userManager.Create(lector5, "lector1234");
            userManager.AddToRole(lector5.Id, "Lector");

            var lector6 = new ApplicationUser {
                UserName = "lector6",
                Email = "lector6@lector.com",
                Age = 36,
                FirstName = "Aster",
                LastName = "Richy",
                UserIsBlocked = false
            };
            userManager.Create(lector6, "lector1234");
            userManager.AddToRole(lector6.Id, "Lector");
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var courses = new List<Course>{
                new Course("Geometry", new DateTime(2019,1,31), new DateTime(2019,5,31), "Mathematics"),
                new Course("Grammar", new DateTime(2019,2,1), new DateTime(2019,7,22), "English"),
                new Course("Space", new DateTime(2019,1,10), new DateTime(2019,6,25), "Physics"),
                new Course("Sound", new DateTime(2019,1,13), new DateTime(2019,4,1), "Physics"),
                new Course("Java", new DateTime(2019,3,25), new DateTime(2019,6,25), "Programming"),
                new Course("C#", new DateTime(2019,4,1), new DateTime(2019,7,31), "Programming"),
                new Course("Python", new DateTime(2018,10,31), new DateTime(2019,1,31), "Programming"),
                new Course("Algebra", new DateTime(2018,11,1), new DateTime(2019,2,28), "Mathematics"),
                new Course("Speaking practice", new DateTime(2019,1,8), new DateTime(2019,3,31), "English"),
                new Course("Elementary", new DateTime(2019,1,10), new DateTime(2019,6,25), "French"),
                new Course("Advanced", new DateTime(2019,6,1), new DateTime(2019,8,31), "French"),
                new Course("Advanced", new DateTime(2018,10,1), new DateTime(2018,12,31), "English")
            };

            courses.ForEach(course => context.Courses.Add(course));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            SeedRoles(roleManager);

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            SeedUsers(userManager);



            context.SaveChanges();
        }

        //Method to register users and assign lectors
        public void RegisterUsersToCoursesAndAddLectors(ApplicationDbContext context)
        {
            UsersManager usersManager = new UsersManager();
            CoursesManager coursesManager = new CoursesManager();
            JournalsManager journalsManager = new JournalsManager();

            List<ApplicationUser> users = usersManager.GetUsers().ToList();
            List<Course> courses = coursesManager.GetCourses().ToList();
            var user1 = users.ElementAt(0);
            var course1 = courses.ElementAt(0);
            string result = coursesManager.AddUserToCourse(course1.Id, user1.Id);
            journalsManager.AddJournalForUser(course1.Id, user1.Id);
            course1.LectorId = usersManager.GetLectorsForCourseEdit(null).LastOrDefault().Id;
        }
    }
}
