using Faculty.Logic.DB;
using Faculty.Logic.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Faculty.Models
{
    public class JournalViewModel
    {
        public int JournalId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public int Mark { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Theme")]
        public string CourseTheme { get; set; }

        [Display(Name = "Course status")]
        public string CourseStatus { get; set; }

        public string Lector { get; set; }

        public int CourseId { get; set; }

        public JournalViewModel()
        {

        }

        public JournalViewModel(int journalId, string firstName, string lastName, int mark, string courseName, int courseId)
        {
            JournalId = journalId;
            FirstName = firstName;
            LastName = lastName;
            Mark = mark;
            CourseName = courseName;
            CourseId = courseId;
        }

        public JournalViewModel(int journalId, string firstName, string lastName, int mark, string courseName, string courseTheme, int courseId, string lector, string courseStatus)
        {
            JournalId = journalId;
            FirstName = firstName;
            LastName = lastName;
            Mark = mark;
            CourseName = courseName;
            CourseId = courseId;
            CourseTheme = courseTheme;
            Lector = lector;
            CourseStatus = courseStatus;
        }

        public static List<JournalViewModel> GetJournalsList(ICollection<ApplicationUser> users, Course course)
        {
            LogManager logManager = new LogManager();
            logManager.AddEventLog("JournalViewModel => GetJournalsList 1st method called", "Method");
            List<JournalViewModel> result = new List<JournalViewModel>();
            foreach (var item in users)
            {
                result.Add(new JournalViewModel(
                   item.Journals.First().Id,
                   item.FirstName,
                   item.LastName,
                   item.Journals.First().Mark,
                   course.CourseName,
                   course.Id
                   ));
            }
            return result;
        }

        public static List<JournalViewModel> GetJournalsList(ICollection<Journal> journals, UsersManager usersManager, CoursesManager coursesManager)
        {
            LogManager logManager = new LogManager();
            logManager.AddEventLog("JournalViewModel => GetJournalsList 2nd method called", "Method");
            
            List<JournalViewModel> result = new List<JournalViewModel>();
            foreach (var item in journals)
            {
                var course = coursesManager.GetSpecificCourse(item.CourseId);
                var lectorData = usersManager.GetSpecificUser(course.LectorId);
                var lector = "None";
                if (lectorData != null)
                    lector = string.Concat(lectorData.FirstName, " ", lectorData.LastName);
                result.Add(new JournalViewModel(
                    item.Id,
                    item.Users.First().FirstName,
                    item.Users.First().LastName,
                    item.Mark,
                    course.CourseName,
                    course.Theme,
                    course.Id,
                    lector,
                    course.CourseStatus.ToString()
                    ));
            }
            return result;
        }

        //get sorted viewmodel list based on filters
        public static List<JournalViewModel> GerSortedJournalsList(string courseName, string courseStatus, List<JournalViewModel> journals)
        {
            LogManager logManager = new LogManager();
            logManager.AddEventLog("JournalViewModel => GerSortedJournalsList method called", "Method");
            if (courseName != null && courseName != "")
            {
                journals = journals
                    .Where(c => c.CourseName.Length >= courseName.Length)
                    .Where(c => c.CourseName.ToLower().Substring(0, courseName.Length) == courseName.ToLower())
                    .ToList();
            }
            if (courseStatus != null && courseStatus != "" && courseStatus != "All")
            {
                journals = journals
                    .Where(c => c.CourseStatus == courseStatus)
                    .ToList();
            }

            return journals;
        }


    }
}