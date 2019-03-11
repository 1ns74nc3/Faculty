using Faculty.Logic.Models;
using System.ComponentModel.DataAnnotations;

namespace Faculty.Logic.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Role { get; set; }
    }
}