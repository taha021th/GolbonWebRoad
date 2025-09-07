namespace GolbonWebRoad.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
