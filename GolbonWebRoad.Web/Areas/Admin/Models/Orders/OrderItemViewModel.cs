using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Orders
{
    public class OrderItemViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "نام محصول")]
        public string ProductName { get; set; }

        [Display(Name = "تعداد")]
        public int Quantity { get; set; }

        [Display(Name = "قیمت واحد")]
        public decimal Price { get; set; }

        public decimal TotalPrice => Quantity * Price;
    }
}
