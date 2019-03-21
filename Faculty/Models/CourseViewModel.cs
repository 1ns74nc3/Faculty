using Faculty.Logic.DB;
using Faculty.Logic.Models;
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

        //return typed list to display it in View
        public static List<CourseViewModel> GetCoursesList(ICollection<Course> coursesList, int constructorType)
        {
            LogManager logManager = new LogManager();
            logManager.AddEventLog("CourseViewModel => GetCoursesList method called", "Method");
            UsersManager usersManager = new UsersManager();
            var lectors = usersManager.GetAllLectors();
            List<CourseViewModel> courses = new List<CourseViewModel>();
            switch (constructorType)
            {
                //Display courses for all users
                case 1:
                    foreach (var item in coursesList)
                    {
                        var lectorData = lectors.Where(u => u.Id == item.LectorId).FirstOrDefault();
                        var lector = "None";
                        if (lectorData != null)
                            lector = string.Concat(lectorData.FirstName, " ", lectorData.LastName);
                        courses.Add(new CourseViewModel(
                            item.Id, 
                            item.CourseName, 
                            item.StartDate.ToShortDateString(), 
                            item.EndDate.ToShortDateString(),
                            item.Theme, 
                            item.CourseStatus.ToString(), 
                            item.Users.Count, 
                            lector));
                    }
                    break;
                //Display courses for lector
                case 2:
                    foreach (var item in coursesList)
                    {
                        courses.Add(new CourseViewModel(
                            item.Id,
                            item.CourseName,
                            item.StartDate.ToShortDateString(),
                            item.EndDate.ToShortDateString(),
                            item.Theme,
                            item.CourseStatus.ToString(),
                            item.Users.Count));
                    }
                    break;
            }
            
            return courses;
        }
    }
}