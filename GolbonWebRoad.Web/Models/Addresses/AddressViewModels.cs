using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Models.Addresses
{
    public class AddressListViewModel
    {
        public List<AddressItemViewModel> Items { get; set; } = new List<AddressItemViewModel>();
    }

    public class AddressItemViewModel
    {
        public int Id { get; set; }
        public string UserFullName { get; set; } // نام کامل کاربر از User entity
        public string Phone { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public bool IsDefault { get; set; }
    }

    public class AddressFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "شماره تماس")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "آدرس کامل")]
        public string AddressLine { get; set; }

        [Required]
        [Display(Name = "شهر")]
        public string City { get; set; }

        [Required]
        [Display(Name = "استان")]
        public string Province { get; set; }

        [Required]
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "به عنوان پیش‌فرض")]
        public bool IsDefault { get; set; }
    }
}


