using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Faculty.Logic.Models
{
    public class Course : ModelsBase
    {
        [Required]
        [Display(Name = "Course")]
        public string CourseName { get; set; }
        [Required]
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required]
        public string Theme { get; set; }

        public enum Status
        {
            Unknown = 0,
            Upcoming = 1,
            Active = 2,
            Ended = 3
        }

        [Display(Name = "Status")]
        public Status CourseStatus { get; set; }

        [Display(Name = "Description")]
        public string CourseDescription { get; set; }

        public string LectorId { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Course()
        {
            Users = new HashSet<ApplicationUser>();
        }

        public Course(string courseName, DateTime startDate, DateTime endDate, string theme)
        {
            CourseName = courseName;
            StartDate = startDate;
            EndDate = endDate;
            CourseStatus = SetStatus(startDate, endDate);
            Theme = theme;
            Users = new HashSet<ApplicationUser>();
        }

        public Status SetStatus(DateTime startDate, DateTime endDate)
        {
            Status result = Status.Unknown;
            if (DateTime.Compare(startDate, DateTime.Now) > 0)
            {
                result = Status.Upcoming;
            }
            else if (DateTime.Compare(endDate, DateTime.Now) < 0)
            {
                result = Status.Ended;
            }
            else
            {
                result = Status.Active;
            }
            return result;
        }
    }
}

