using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Faculty.Logic.Models.Course;

namespace Faculty.Models
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Course")]
        public string CourseName { get; set; }

        [Display(Name = "Start date")]
        public string StartDate { get; set; }

        [Display(Name = "End date")]
        public string EndDate { get; set; }

        public string Theme { get; set; }

        [Display(Name = "Status")]
        public string CourseStatus { get; set; }

        [Display(Name = "Students Registered")]
        public int StudentsCount { get; set; }

        public string Lector { get; set; }

        public CourseViewModel()
        {

        }

        public CourseViewModel(int id, string courseName, string startDate, string endDate, string theme, string courseStatus, int studentsCount) { 
            Id = id;
            CourseName = courseName;
            StartDate = startDate;
            EndDate = endDate;
            Theme = theme;
            CourseStatus = courseStatus;
            StudentsCount = studentsCount;
        }

        public CourseViewModel(int id, string courseName, string startDate, string endDate, string theme, string courseStatus, int studentsCount, string lector)
        {
            Id = id;
            CourseName = courseName;
            StartDate = startDate;
            EndDate = endDate;
            Theme = theme;
            CourseStatus = courseStatus;
            StudentsCount = studentsCount;
            Lector = lector;
        }

    }
}