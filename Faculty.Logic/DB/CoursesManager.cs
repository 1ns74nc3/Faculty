using Faculty.Logic.Models;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Faculty.Logic.DB
{
    //Get, edit, delete data from Courses table
    public class CoursesManager
    {
        private UsersManager usersManager = new UsersManager();
        private JournalsManager journalsManager = new JournalsManager();
        private LogManager logManager = new LogManager();

        //Add new course to database
        public void AddCourse(Course course)
        {
            logManager.AddEventLog("CoursesManager => AddCourse method called", "Method");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (course != null)
                {
                    Course tempCourse = new Course(course.CourseName, course.StartDate, course.EndDate, course.Theme);
                    tempCourse.LectorId = course.LectorId;
                    tempCourse.CourseDescription = course.CourseDescription;
                    tempCourse.SetStatus();
                    db.Courses.Add(tempCourse);
                    db.SaveChanges();
                }
            }
        }

        //Sign user to specific course
        public string AddUserToCourse(int courseId, string userId)
        {
            logManager.AddEventLog("CoursesManager => AddUserToCourse method called", "Method");
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var course = db.Courses.SingleOrDefault(c => c.Id == courseId);
                    var user = db.Users.SingleOrDefault(c => c.Id == userId);
                    if (!course.Users.Contains(user))
                    {
                        course.Users.Add(user);
                        db.SaveChanges();
                        return "Congratulations! You signed to the course!";
                    }
                    return "You already signed to this course!";
                }
        }

        //Get list of all themes
        public List<string> GetAllThemes(string currentTheme)
        {
            logManager.AddEventLog("CoursesManager => GetAllThemes method called", "Method");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<string> result = new List<string> { currentTheme };
                foreach(var item in db.Courses.Select(c => c.Theme).Distinct().ToList())
                {
                    if (!result.Contains(item))
                    {
                        result.Add(item);
                    }
                }
                if (currentTheme != null && currentTheme != "")
                {
                    result.Add(null);
                }
                return result;
            }
        }

        //Return sorted list of courses using filters
        public ICollection<Course> GetSortedCourses(string currentFilter, string status, string theme, string lector, string courseName, ICollection<Course> courses)
        {
            logManager.AddEventLog("CoursesManager => GetSortedCourses method called", "Method");
            if (status != null && status != "" && status != "All")
            {
                courses = courses
                    .Where(c => c.CourseStatus.ToString() == status)
                    .ToList();
            }

            if (theme != null && theme != "")
            {
                courses = courses
                    .Where(c => c.Theme == theme)
                    .ToList();
            }

            if (courseName != null && courseName != "")
            {
                courses = courses
                    .Where(c => c.CourseName.Length >= courseName.Length)
                    .Where(c => c.CourseName.ToLower().Substring(0, courseName.Length) == courseName.ToLower())
                    .ToList();
            }

            if (lector!=null && lector!="")
            {
                string lectorId = null;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var users = db.Users.Where(u => u.Roles.Any(r => r.RoleId == db.Roles.FirstOrDefault(role => role.Name == "Lector").Id)).ToList();
                    lectorId = users.Where(u => string.Concat(u.FirstName," ",u.LastName).Equals(lector)).FirstOrDefault().Id;
                }
                courses = courses.Where(c => c.LectorId == lectorId).ToList();
            }

            switch (currentFilter)
            {
                case "a-z":
                    return courses.OrderBy(c => c.CourseName).ToList();
                case "z-a":
                    return courses.OrderByDescending(c => c.CourseName).ToList();
                //ShortestToHighest
                case "durationSTH":
                    return courses.OrderBy(c => c.EndDate.Subtract(c.StartDate)).ToList();
                //HighestToShortest
                case "durationHTS":
                    return courses.OrderByDescending(c => c.EndDate.Subtract(c.StartDate)).ToList();
                //ShortestToHighest
                case "studentsCountSTH":
                    return courses.OrderBy(c => c.Users.Count).ToList();
                //HighestToShortest
                case "studentsCountHTS":
                    return courses.OrderByDescending(c => c.Users.Count).ToList();
                default: return courses;
            }
        }

        //Delete course and all related journals from database
        public void DeleteCourse(int courseId)
        {
            logManager.AddEventLog("CoursesManager => DeleteCourse method called", "Method");
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var course = db.Courses.FirstOrDefault(c => c.Id == courseId);
                    journalsManager.DeleteJournalsWhenRemovingCourse(courseId);
                    db.Courses.Remove(course);
                    db.SaveChanges();
                }

        }

        //Edit course in database
        public void EditCourse(Course course)
        {
            logManager.AddEventLog("CoursesManager => EditCourse method called", "Method");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var currentCourse = db.Courses.SingleOrDefault(c => c.Id == course.Id);
                currentCourse.CourseName = course.CourseName;
                currentCourse.StartDate = course.StartDate;
                currentCourse.EndDate = course.EndDate;
                currentCourse.LectorId = course.LectorId;
                currentCourse.Theme = course.Theme;
                currentCourse.CourseDescription = course.CourseDescription;
                currentCourse.SetStatus();
                
                db.SaveChanges();
            }
        }

        //Get list of all courses
        public ICollection<Course> GetCourses()
        {
            logManager.AddEventLog("CoursesManager => GetCourses method called", "Method");
            List<Course> courses = new List<Course>();
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                courses = db.Courses.Include(u => u.Users).ToList();
            }
            return courses;
        }

        //Get list of all courses for specific lector
        public ICollection<Course> GetCoursesForLector(string userId)
        {
            logManager.AddEventLog("CoursesManager => GetCoursesForLector method called", "Method");
            List<Course> courses = new List<Course>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                courses = db.Courses.Where(c => c.LectorId == userId).Include(u => u.Users).ToList();
            }
            return courses;
        }

        //Get information about Lector of the course
        public string GetLectorInfo(Course course)
        {
            logManager.AddEventLog("CoursesManager => GetLectorInfo method called", "Method");
            if (course.LectorId != null && course.LectorId != "")
            {
                var lector = usersManager.GetSpecificUser(course.LectorId);
                return string.Concat(lector.FirstName, " ", lector.LastName);
            }
            return "There's no lector for this course!";
        }

        //Get specific course by Course ID
        public Course GetSpecificCourse(int courseId)
        {
            logManager.AddEventLog("CoursesManager => GetSpecificCourse method called", "Method");
                Course course = new Course();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    course = db.Courses.ToList().Where(c => c.Id == courseId).FirstOrDefault();
                }
                return course;
        }

        //Remove user from course
        public string RemoveUserFromCourse(int courseId, string userId)
        {
            logManager.AddEventLog("CoursesManager => RemoveUserFromCourse method called", "Method");
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var course = db.Courses.SingleOrDefault(c => c.Id == courseId);
                    var user = db.Users.SingleOrDefault(c => c.Id == userId);
                    if (course.Users.Contains(user))
                    {
                        course.Users.Remove(user);
                        db.SaveChanges();
                        return "You left the course!";
                    }
                    return "You are not registered to this course!";
                }
        }

        //Check if user is signed to the course
        public bool UserIsSignedToCourse(int courseId, string userId)
        {
            logManager.AddEventLog("CoursesManager => UserIsSignedToCourse method called", "Method");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                    var course = db.Courses.SingleOrDefault(c => c.Id == courseId);
                    var user = db.Users.SingleOrDefault(c => c.Id == userId);
                    return course.Users.Contains(user);
             }
        }
    }
}
