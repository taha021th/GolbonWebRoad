using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Domain.Entities
{
    public class User : ApplicationUser
    {
        [Required]
        public long MobileNumber { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<UserAddress> Addresses { get; set; }
    }
}