namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{

    public class DeleteProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
    }
}
