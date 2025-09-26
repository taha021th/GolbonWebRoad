using GolbonWebRoad.Application.Dtos.Products;

namespace GolbonWebRoad.Application.Dtos.CartItems
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // آبجکت کامل محصول را در خود نگه می‌دارد
        public ProductDto Product { get; set; }

        // ویژگی‌های واریانت را برای نمایش در سبد خرید نگه می‌دارد
        public Dictionary<string, string> VariantAttributes { get; set; } = new Dictionary<string, string>();
    }
}
