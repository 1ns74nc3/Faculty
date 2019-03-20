using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Faculty.Logic.Models
{
    public class Course : ModelsBase
    {
        [Required]
        [Display(Name = "Course Name")]
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

        [Display(Name = "Course Status")]
        public Status CourseStatus { get; set; }

        [Display(Name = "Course Description")]
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
            if (startDate > endDate)
            {
                EndDate = startDate.AddDays(90);
            }
            else
            {
                EndDate = endDate;
            }
            SetStatus();
            Theme = theme;
            Users = new HashSet<ApplicationUser>();
        }

        public void SetStatus()
        {
            if (LectorId == "" || LectorId == null)
            {
                CourseStatus = Status.Unknown;
            }
            else
            {
                CourseStatus = Status.Unknown;
                if (DateTime.Compare(StartDate, DateTime.Now) > 0)
                {
                    CourseStatus = Status.Upcoming;
                }
                else if (DateTime.Compare(EndDate, DateTime.Now) < 0)
                {
                    CourseStatus = Status.Ended;
                }
                else
                {
                    CourseStatus = Status.Active;
                }
            }
            
        }
    }
}

