using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Application.Dtos.Products
{
    public class UpdateProductRequestDto
    {
        [Required(ErrorMessage = "شناسه محصول الزامی است.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Slog الزامی است.")]
        public string Slog { get; set; }
        [Required(ErrorMessage = "نام الزامی است.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "توضیحات الزامی است.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "قیمت الزامی است.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "موجودی الزامی است.")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "شناسه دسته بندی الزامی است.")]
        public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
