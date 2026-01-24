using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    public class Faq
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string? Slog { get; set; } // اسلاگ یکتا برای لینک/Anchor

        [Required]
        [MaxLength(500)]
        public string Question { get; set; } = default!;

        [Required]
        public string Answer { get; set; } = default!; // HTML مجاز

        // --- New relation to FaqCategory ---
        public int? FaqCategoryId { get; set; }
        [ForeignKey(nameof(FaqCategoryId))]
        public virtual FaqCategory? Category { get; set; }

        [MaxLength(500)]
        public string? Tags { get; set; } // رشته کاما-سِپَرِیت

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
