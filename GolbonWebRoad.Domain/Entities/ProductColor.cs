namespace GolbonWebRoad.Domain.Entities
{
    public class ProductColor
    {
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public virtual Color Color { get; set; }
    }

}
