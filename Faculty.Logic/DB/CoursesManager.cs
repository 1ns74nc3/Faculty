using Faculty.Logic.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    //Get, edit, delete data from Courses table
    public class CoursesManager
    {
        //Add new course to database
        public void AddCourse(Course course)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (course != null)
                {
                    Course tempCourse = new Course(course.CourseName, course.StartDate, course.EndDate, course.Theme);
                    tempCourse.CourseDescription = course.CourseDescription;
                    db.Courses.Add(tempCourse);
                    db.SaveChanges();
                }
            }
        }

        //Sign user to specific course
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

        //Delete course and all related journals from database
        public void DeleteCourse(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                JournalsManager journalsManager = new JournalsManager();
                var course = db.Courses.FirstOrDefault(c => c.Id.Equals(id));
                journalsManager.DeleteJournalsWhenRemovingCourse(id);
                db.Courses.Remove(course);
                db.SaveChanges();
            }
        }

        //Edit course in database
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
        //End course
        public void EndCourse(int courseId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var course = db.Courses.SingleOrDefault(c => c.Id == courseId);
                course.CourseStatus = Course.Status.Ended;
                db.Entry(course).CurrentValues.SetValues(course);
                db.SaveChanges();
            }
        }

        //Get list of all courses
        public ICollection<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                courses = db.Courses.Include(u => u.Users).ToList();
            }
            return courses;
        }

        //Get information about Lector of the course
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

        //Get specific course by Course ID
        public Course GetSpecificCourse(int id)
        {
            Course course = new Course();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                course = db.Courses.ToList().Where(c => c.Id == id).FirstOrDefault();
            }
            return course;
        }

        //Unsign user from course
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

        //Check if user is signed to course
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
