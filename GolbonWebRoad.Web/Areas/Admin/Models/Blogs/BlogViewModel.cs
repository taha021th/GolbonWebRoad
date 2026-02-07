using GolbonWebRoad.Web.Models.BlogReviews;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Blogs
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; } // خلاصه مطلب
        public string Content { get; set; } // متن اصلی        
        public bool IsPublished { get; set; } // وضعیت انتشار
        public string? MainImageUrl { get; set; }
        public DateTime PublishDate { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        public List<BlogReviewViewModel> BlogReviewViewModels { get; set; }
    }
}
