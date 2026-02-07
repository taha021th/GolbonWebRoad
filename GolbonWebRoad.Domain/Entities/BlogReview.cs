using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    public class BlogReview
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public int BlogId { get; set; }
        public Blog Blogs { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public bool Status { get; set; } = false;
    }
}
