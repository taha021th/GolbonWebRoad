namespace GolbonWebRoad.Application.Dtos.Faqs
{
    public class FaqCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Slog { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
