namespace GolbonWebRoad.Web.Areas.Admin.Models.ProductAttributeValue
{
    public class ProductAttributeValueIndexViewModel
    {
        public List<ProductAttributeValueRowViewModel> Items { get; set; } = new List<ProductAttributeValueRowViewModel>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        // Context (filter) info
        public int? AttributeId { get; set; }
        public string? AttributeName { get; set; }
    }

    public class ProductAttributeValueRowViewModel
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Value { get; set; }
    }
}
