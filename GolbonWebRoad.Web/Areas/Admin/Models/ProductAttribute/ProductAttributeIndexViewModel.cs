namespace GolbonWebRoad.Web.Areas.Admin.Models.ProductAttribute
{
    public class ProductAttributeIndexViewModel
    {
        public List<ProductAttributeViewModel> Items { get; set; } = new List<ProductAttributeViewModel>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}