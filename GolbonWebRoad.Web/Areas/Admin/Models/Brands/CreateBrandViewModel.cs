namespace GolbonWebRoad.Web.Areas.Admin.Models.Brands
{
    public class CreateBrandViewModel
    {
        public string Name { get; set; }
        public string? Slog { get; set; }
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}
