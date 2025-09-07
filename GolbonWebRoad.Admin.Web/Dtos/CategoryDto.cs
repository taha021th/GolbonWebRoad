using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Admin.Web.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "نام دسته بندی الزامی است.")]
        [Display(Name = "نام دسته بندی")]
        public string Name { get; set; }
    }
}
