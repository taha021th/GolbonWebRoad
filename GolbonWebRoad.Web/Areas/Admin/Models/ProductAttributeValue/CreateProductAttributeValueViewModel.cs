using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Web.Areas.Admin.Models.ProductAttributeValue
{
    public class CreateProductAttributeValueViewModel
    {
        [Display(Name = "ویژگی")]
        [Range(1, int.MaxValue, ErrorMessage = "لطفاً {0} را انتخاب کنید.")]
        public int AttributeId { get; set; }

        [Display(Name = "مقدار")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [MaxLength(100, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string Value { get; set; }

        public SelectList? AttributeOptions { get; set; }
    }
}
