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
        [Range(0,120)]
        public int Age { get; set; }
        [Required]
        [Display(Name = "User status")]
        public bool UserIsBlocked { get; set; }

        [Display(Name = "Information(Optional)")]
        public string UserInformation { get; set; }

        public virtual ICollection<Course> Courses { get; set; }

        public virtual ICollection<Journal> Journals { get; set; }

        public ApplicationUser()
        {
            Journals = new HashSet<Journal>();
            Courses = new HashSet<Course>();
        }
    }

}