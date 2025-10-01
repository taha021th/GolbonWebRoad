using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.ProductAttribute
{
    public class UpdateProductAttributeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "نام ویژگی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [MaxLength(100, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string Name { get; set; }
    }
}
