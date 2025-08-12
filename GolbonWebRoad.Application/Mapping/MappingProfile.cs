using AutoMapper;
using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductCommand, Product>();

        }
    }
}
