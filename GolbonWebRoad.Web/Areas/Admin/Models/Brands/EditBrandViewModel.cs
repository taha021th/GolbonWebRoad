namespace GolbonWebRoad.Web.Areas.Admin.Models.Brands
{
    public class EditBrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ExistingImage { get; set; }
        public IFormFile? NewImage { get; set; }
    }
}
