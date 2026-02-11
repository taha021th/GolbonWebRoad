namespace GolbonWebRoad.Web.Areas.Admin.Models.BlogReviews
{
    public class BlogReviewViewModel
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool Status { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string BlogName { get; set; }
        public int BlogId { get; set; }
        public string BlogImageUrl { get; set; }

    }

    public class BlogReviewIndexViewModel
    {
        public List<BlogReviewViewModel> Reviews { get; set; } = new List<BlogReviewViewModel>();
        public int TotalReviews { get; set; }
        public int PendingReviews { get; set; }
        public int ApprovedReviews { get; set; }
        public string FilterStatus { get; set; } = "all";
    }
}
