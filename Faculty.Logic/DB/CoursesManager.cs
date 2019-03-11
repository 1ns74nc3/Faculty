using Faculty.Logic.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    //Get, edit, delete data from Courses table
    public class CoursesManager
    {
        public void AddCourse(Course course)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (course != null)
                {
                    Course tempCourse = new Course(course.CourseName, course.StartDate, course.EndDate);
                    tempCourse.CourseDescription = course.CourseDescription;
                    db.Courses.Add(tempCourse);
                    db.SaveChanges();
                }
            }
        }

        public string AddUserToCourse(int courseId, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var course = db.Courses.SingleOrDefault(c => c.Id == courseId);
                var user = db.Users.SingleOrDefault(c => c.Id == userId);
                if (!course.Users.Contains(user))
                {
                    course.Users.Add(user);
                    db.SaveChanges();
                    return "User added to the course!";
                }
                return "User is already signed to the course!";
            }
        }

        public void DeleteCourse(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var course = db.Courses.FirstOrDefault(c => c.Id.Equals(id));
                db.Courses.Remove(course);
                db.SaveChanges();
            }
        }

        public void EditCourse(Course course)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var currentCourse = db.Courses.SingleOrDefault(c => c.Id == course.Id);
                course.CourseStatus = currentCourse.CourseStatus;
                db.Entry(currentCourse).CurrentValues.SetValues(course);
                db.SaveChanges();
            }
        }


        public ICollection<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                courses = db.Courses.ToList();
            }
            return courses;
        }

        public string GetLectorInfo(Course course)
        {
            if (course.LectorId != null && course.LectorId != "")
            {
                UsersManager userManager = new UsersManager();
                var lector = userManager.GetSpecificUser(course.LectorId);
                return string.Concat(lector.FirstName, " ", lector.LastName);
            }
            return "There's no lector for this course.";
        }

        public Course GetSpecificCourse(int id)
        {
            Course course = new Course();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                course = db.Courses.ToList().Where(c => c.Id == id).FirstOrDefault();
            }
            return course;
        }

        public string RemoveUserFromCourse(int courseId, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var course = db.Courses.SingleOrDefault(c => c.Id == courseId);
                var user = db.Users.SingleOrDefault(c => c.Id == userId);
                if (course.Users.Contains(user))
                {
                    course.Users.Remove(user);
                    db.SaveChanges();
                    return "User removed from the course!";
                }
                return "You are not registered to this course!";
            }
        }

        public bool UserIsSignedToCourse(int courseId, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var course = db.Courses.SingleOrDefault(c => c.Id == courseId);
                var user = db.Users.SingleOrDefault(c => c.Id == userId);
                return course.Users.Contains(user);
            }
        }
    }
}
