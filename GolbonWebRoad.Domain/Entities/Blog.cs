using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    public class Blog
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string? ShortDescription { get; set; }
        public string? MainImageUrl { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;


        //seo
        [MaxLength(256)]
        public string? Slog { get; set; }
        [MaxLength(100)]
        public string? MetaTitle { get; set; }

        [MaxLength(200)]
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MainImageAltText { get; set; }
        public string? CanonicalUrl { get; set; }

        public string? H1Title { get; set; }
        [MaxLength(50)]
        public string? MetaRobots { get; set; }

        public bool IsShowHomePage { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual BlogCategory BlogCategory { get; set; }
        public virtual ICollection<BlogReview> BlogReviews { get; set; }
    }

}
