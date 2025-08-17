using AutoMapper;
using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Domain.Entities;
namespace GolbonWebRoad.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Products
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductCommand, Product>();
            CreateMap<UpdateProductCommand, Product>();
            CreateMap<Product, ProductSummaryDto>();



            //Categories
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategorySummaryDto>().ReverseMap();
            CreateMap<CategoryDto, CategorySummaryDto>();
            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<UpdateCategoryCommand, Category>();
            CreateMap<GetCategoriesQuery, Category>();

            //Orders
            CreateMap<Order, OrderDto>().ReverseMap();


        }
    }
}