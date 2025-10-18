using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Orders
{
    public class UserAddressViewModel
    {
        public int Id { get; set; }

        [Display(Name = "شماره تماس")]
        public string Phone { get; set; }

        [Display(Name = "آدرس کامل")]
        public string AddressLine { get; set; }

        [Display(Name = "شهر")]
        public string City { get; set; }

        [Display(Name = "استان")]
        public string Province { get; set; }

        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "آدرس پیش‌فرض")]
        public bool IsDefault { get; set; }
    }
}