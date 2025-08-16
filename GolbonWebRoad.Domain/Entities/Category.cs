namespace GolbonWebRoad.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
