using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Orders
{
    public class OrderIndexViewModel
    {
        [Display(Name = "شماره سفارش")]
        public int Id { get; set; }

        [Display(Name = "نام کاربری مشتری")]
        public string UserName { get; set; }

        [Display(Name = "تاریخ سفارش")]
        public string OrderDate { get; set; }

        [Display(Name = "مبلغ کل")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "وضعیت")]
        public string OrderStatus { get; set; }
    }
}
