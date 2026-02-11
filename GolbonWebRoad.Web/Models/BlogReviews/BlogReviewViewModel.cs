using GolbonWebRoad.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Models.BlogReviews
{
    public class BlogReviewViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "متن کامنت الزامی است.")]
        public string ReviewText { get; set; }
        [Required(ErrorMessage = "انتخاب ستاره برای ثبت میزان رضایت شما الزامی است.")]
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        [Required(ErrorMessage = "شناسه بلاگ الزامی است.")]
        public int BlogId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool Status { get; set; }
    }
}
