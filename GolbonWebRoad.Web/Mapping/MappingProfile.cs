using AutoMapper;
using GolbonWebRoad.Application.Dtos.ProductImages;
using GolbonWebRoad.Application.Dtos.Users;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Users.Commands;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Web.Areas.Admin.Models.Categories;
using GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels;
using GolbonWebRoad.Web.Areas.Admin.Models.Users;

namespace GolbonWebRoad.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Product
            #region admin
            CreateMap<Product, ProductViewModel>()
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.MapFrom(src =>
                    src.Images
                       .Where(img => img.IsMainImage == true)
                       .Select(img => img.ImageUrl)
                       .FirstOrDefault()
                )
            );
            CreateMap<CreateProductViewModel, CreateProductCommand>();
            CreateMap<Product, EditProductViewModel>().ForMember(dest => dest.ExistingColors, opt => opt.MapFrom(src => src.ProductColors.Select(pc => new ExistingColorViewModel { Id=pc.Color.Id, Name=pc.Color.Name, HexCode=pc.Color.HexCode }).ToList()));
            CreateMap<EditProductViewModel, UpdateProductCommand>();
            CreateMap<ProductImages, ProductImageDto>();
            CreateMap<Product, DeleteProductViewModel>();
            #endregion

            #region Ui

            #endregion
            #endregion


            #region Category 
            #region admin
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CreateCategoryViewModel, CreateCategoryCommand>();
            CreateMap<Category, EditCategoryViewModel>()
                .ForMember(dest => dest.ExistingImage, opt => opt.MapFrom(src => src.ImageUrl));
            CreateMap<EditCategoryViewModel, UpdateCategoryCommand>();

            #endregion

            #region Ui

            #endregion
            #endregion


            #region User
            CreateMap<ManageUserRolesDto, ManageUserRolesViewModel>().ReverseMap();
            // این مپ جایگزین مپ اشتباه قبلی می‌شود
            // این مپ هم برای نمایش و هم برای ارسال اطلاعات کار می‌کند
            CreateMap<RoleDto, RoleViewModel>().ReverseMap();
            // نقشه برای آپدیت کردن نقش‌ها
            CreateMap<ManageUserRolesViewModel, UpdateUserRoleCommand>();
            #endregion



        }
    }
}
