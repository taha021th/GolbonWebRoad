﻿using Microsoft.AspNetCore.Http;

namespace GolbonWebRoad.Application.Dtos.Products
{
    public class UpdateProductRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
