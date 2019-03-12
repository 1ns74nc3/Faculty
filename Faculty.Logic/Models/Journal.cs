using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Faculty.Logic.Models
{
    public class Journal : ModelsBase
    {
        [Required]
        [Range(0, 100)]
        public byte Mark { get; set; }

        [Required]
        public int CourseId { get; set; }

        public string UserId { get; set; }

        public Journal()
        {

        }

        public Journal(int courseId, string userId)
        {
            Mark = 0;
            CourseId = courseId;
            UserId = userId;
        }

    }
}
