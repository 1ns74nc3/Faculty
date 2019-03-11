using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Faculty.Logic.Models
{
    public class Journal : ModelsBase
    {
        [Required]
        [Range(0, 100)]
        public byte Mark { get; set; }

        public ICollection<Course> Courses { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Journal()
        {
            Mark = 0;
            Courses = new HashSet<Course>();
            Users = new HashSet<ApplicationUser>();
        }

    }
}
