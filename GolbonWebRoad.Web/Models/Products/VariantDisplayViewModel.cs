namespace GolbonWebRoad.Web.Models.Products
{
    public class VariantDisplayViewModel
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Gtin { get; set; }
        public string? Mpn { get; set; }
        public List<int> AttributeValueIds { get; set; } = new List<int>();
    }

    public class AttributeGroupDisplayViewModel
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public List<AttributeValueDisplayViewModel> Values { get; set; } = new List<AttributeValueDisplayViewModel>();
    }

    public class AttributeValueDisplayViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
