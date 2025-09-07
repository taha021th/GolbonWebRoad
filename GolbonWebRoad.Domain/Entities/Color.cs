namespace GolbonWebRoad.Domain.Entities
{
    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HexCode { get; set; }
        public virtual ICollection<ProductColor> ProductColors { get; set; }
    }
}
