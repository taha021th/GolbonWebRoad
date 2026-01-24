using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    public class BlogCategory
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        public string? Slog { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? AltTextImageUrl { get; set; }
        [MaxLength(100)]
        public string? MetaTitle { get; set; }
        [MaxLength(200)]
        public string? MetaDescription { get; set; }
        [MaxLength(50)]
        public string? MetaRobots { get; set; }
        public string? CanonicalUrl { get; set; }
        public int? BlogId { get; set; }
        [ForeignKey(nameof(BlogId))]
        public virtual ICollection<Blog> Blogs { get; set; }

    }
}
