using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }
    }
}
