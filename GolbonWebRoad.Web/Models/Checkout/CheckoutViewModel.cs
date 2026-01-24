using GolbonWebRoad.Application.Dtos.Logistics;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Web.Models.Cart;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Models.Checkout
{
    /// <summary>
    /// این ویومدل، نسخه بازنویسی شده برای پردازش صحیح اطلاعات صفحه تسویه حساب است.
    /// منطق اعتبارسنجی آدرس به داخل خود این کلاس منتقل شده است.
    /// </summary>
    public class CheckoutViewModel : IValidatableObject
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public long TotalAmount { get; set; }
        public List<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public List<ShippingQuoteDto> ShippingOptions { get; set; } = new List<ShippingQuoteDto>();

        // بخش ورودی‌های فرم
        [Display(Name = "آدرس انتخاب شده")]
        public int? SelectedAddressId { get; set; }

        [Display(Name = "روش ارسال")]
        [Required(ErrorMessage = "لطفا یک روش ارسال را انتخاب کنید.")]
        public string SelectedShippingMethod { get; set; }

        // فیلدهای آدرس جدید
        [Display(Name = "نام و نام خانوادگی")]
        public string? NewFullName { get; set; }
        [Display(Name = "آدرس کامل")]
        public string? NewAddressLine { get; set; }
        [Display(Name = "شهر")]
        public string? NewCity { get; set; }
        [Display(Name = "استان")]
        public string? NewProvince { get; set; }
        [Display(Name = "کد پستی")]
        public string? NewPostalCode { get; set; }
        [Display(Name = "شماره تماس")]
        public string? NewPhone { get; set; }

        /// <summary>
        /// این متد به صورت خودکار توسط ASP.NET Core فراخوانی می‌شود تا قوانین اعتبارسنجی پیچیده‌تر را بررسی کند.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // بررسی می‌کنیم آیا کاربر یک آدرس موجود را انتخاب کرده است.
            bool hasExistingAddress = SelectedAddressId.HasValue && SelectedAddressId > 0;

            // بررسی می‌کنیم آیا کاربر تمام فیلدهای آدرس جدید را پر کرده است.
            bool hasCompleteNewAddress = !string.IsNullOrWhiteSpace(NewFullName) &&
                                         !string.IsNullOrWhiteSpace(NewAddressLine) &&
                                         !string.IsNullOrWhiteSpace(NewCity) &&
                                         !string.IsNullOrWhiteSpace(NewProvince) &&
                                         !string.IsNullOrWhiteSpace(NewPostalCode) &&
                                         !string.IsNullOrWhiteSpace(NewPhone);

            // اگر هیچکدام از دو حالت بالا برقرار نبود، یک خطای اعتبارسنجی ایجاد کن.
            if (!hasExistingAddress && !hasCompleteNewAddress)
            {
                yield return new ValidationResult(
                    "لطفاً یک آدرس موجود را انتخاب کنید یا تمام فیلدهای آدرس جدید را به صورت کامل پر کنید.",
                    // این نام‌ها به ASP.NET کمک می‌کند تا بفهمد خطا مربوط به کدام فیلدهاست.
                    new[] { nameof(SelectedAddressId), nameof(NewFullName) });
            }
        }
    }
}


