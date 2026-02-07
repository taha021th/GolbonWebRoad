using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Web.Models.BlogReviews
{
    public class BlogReviewViewModel
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool Status { get; set; } = false;
    }
}
