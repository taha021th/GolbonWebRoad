namespace GolbonWebRoad.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
