using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    /// <summary>
    /// محصولات
    /// </summary>
    public class Product
    {
        public Product()
        {

            Reviews=new HashSet<Reviews>();
            Images=new HashSet<ProductImages>();
            ProductColors=new HashSet<ProductColor>();
        }
        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ShrotDescription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int Quantity { get; set; }
        //کد شناسایی محصول مثلا CH-WD-001
        public string? SKU { get; set; }
        public bool IsFeatured { get; set; }

        public int CategoryId { get; set; }
        public int? BrandId { get; set; }

        public Category Category { get; set; }
        public ICollection<Reviews> Reviews { get; set; }
        public ICollection<ProductImages> Images { get; set; }
        public virtual ICollection<ProductColor> ProductColors { get; set; }

        [ForeignKey(nameof(BrandId))]
        public virtual Brand Brand { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
