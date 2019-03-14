using System.ComponentModel.DataAnnotations;

namespace Faculty.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public int Age { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public UserViewModel()
        {

        }

        public UserViewModel(string id, string firstName, string lastName, int age, string email, string roleName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Email = email;
            RoleName = roleName;
        }
    }
}