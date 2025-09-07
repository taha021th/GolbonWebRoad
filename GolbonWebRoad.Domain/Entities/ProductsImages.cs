namespace GolbonWebRoad.Domain.Entities
{
    public class ProductImages
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMainImage { get; set; }
        public int ProductId { get; set; }
    }
}
