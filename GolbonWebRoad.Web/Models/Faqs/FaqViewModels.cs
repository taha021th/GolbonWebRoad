namespace GolbonWebRoad.Web.Models.Faqs
{
    public class FaqItemViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string AnswerHtml { get; set; } = string.Empty;
        public string? Slog { get; set; }
        public string? Tags { get; set; }
    }

    public class FaqCategoryViewModel
    {
        public string Category { get; set; } = "سایر";
        public List<FaqItemViewModel> Items { get; set; } = new();
    }

    public class FaqPageViewModel
    {
        public List<FaqCategoryViewModel> Categories { get; set; } = new();
    }
}
