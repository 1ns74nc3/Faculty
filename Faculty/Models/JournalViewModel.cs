using System.ComponentModel.DataAnnotations;

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

        public JournalViewModel(int journalId, string firstName, string lastName, int mark, string courseName)
        {
            JournalId = journalId;
            FirstName = firstName;
            LastName = lastName;
            Mark = mark;
            CourseName = courseName;
        }

        public JournalViewModel(int journalId, string firstName, string lastName, int mark, string courseName, string courseTheme, int courseId, string lector)
        {
            JournalId = journalId;
            FirstName = firstName;
            LastName = lastName;
            Mark = mark;
            CourseName = courseName;
            CourseId = courseId;
            CourseTheme = courseTheme;
            Lector = lector;
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
    }
}