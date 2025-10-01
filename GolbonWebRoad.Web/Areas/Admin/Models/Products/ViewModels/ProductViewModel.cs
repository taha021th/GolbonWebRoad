namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public decimal BasePrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }

}
