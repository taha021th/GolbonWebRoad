using AutoMapper;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Web.Areas.Admin.Models;
using GolbonWebRoad.Web.Areas.Admin.Models.Categories;

namespace GolbonWebRoad.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductViewModel, CreateProductCommand>();
            CreateMap<CreateProductViewModel, CreateProductCommand>();
            CreateMap<CategoryDto, CategorySummaryDto>().ReverseMap();
            CreateMap<EditProductViewModel, UpdateProductCommand>();
            CreateMap<ProductDto, DeleteProductViewModel>();

            CreateMap<CreateCategoryViewModel, CreateCategoryCommand>();
            CreateMap<CategoryDto, EditCategoryViewModel>();
            CreateMap<EditCategoryViewModel, UpdateCategoryCommand>().ReverseMap();
        }
    }
}
