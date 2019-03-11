using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Faculty.Logic.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        [Display(Name = "User status")]
        public bool UserIsBlocked { get; set; }

        [Display(Name = "Information")]
        public string UserInformation { get; set; }

        public virtual ICollection<Course> Courses { get; set; }

        public virtual ICollection<Journal> Journal { get; set; }

        public ApplicationUser()
        {
            Courses = new HashSet<Course>();
            Journal = new HashSet<Journal>();
        }
    }

}