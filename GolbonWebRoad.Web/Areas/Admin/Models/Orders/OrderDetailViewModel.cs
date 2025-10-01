using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Orders
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }

        [Display(Name = "نام کاربری مشتری")]
        public string UserName { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public string OrderDate { get; set; }

        [Display(Name = "وضعیت سفارش")]
        public string OrderStatus { get; set; }

        [Display(Name = "مبلغ نهایی")]
        public decimal TotalAmount { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        // Shipping address (if any)
        public string AddressFullName { get; set; }
        public string AddressPhone { get; set; }
        public string AddressLine { get; set; }
        public string AddressCity { get; set; }
        public string AddressPostalCode { get; set; }
    }
}
