using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Domain.Entities
{
    public class FaqCategory
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;
        [MaxLength(256)]
        public string? Slog { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Faq> Faqs { get; set; } = new HashSet<Faq>();
    }
}
