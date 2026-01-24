namespace GolbonWebRoad.Application.Dtos.Faqs
{
    public class FaqDto
    {
        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
        public int? FaqCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Tags { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
