using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Reviews
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool Status { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string ProductImageUrl { get; set; }
    }

    public class ReviewIndexViewModel
    {
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
        public int TotalReviews { get; set; }
        public int PendingReviews { get; set; }
        public int ApprovedReviews { get; set; }
        public string FilterStatus { get; set; } = "all"; // all, pending, approved
    }
}
