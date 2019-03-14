using System.ComponentModel.DataAnnotations;

namespace Faculty.Models
{
    public class JournalViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public int Mark { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        public int CourseId { get; set; }

        public JournalViewModel()
        {

        }

        public JournalViewModel(int id, string firstName, string lastName, int mark, string courseName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Mark = mark;
            CourseName = courseName;
        }

        public JournalViewModel(int id, string firstName, string lastName, int mark, string courseName, int courseId)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Mark = mark;
            CourseName = courseName;
            CourseId = courseId;
        }
    }
}