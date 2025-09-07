using AutoMapper;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Application.Dtos.Users;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Users.Commands;
using GolbonWebRoad.Web.Areas.Admin.Models.Categories;
using GolbonWebRoad.Web.Areas.Admin.Models.Products.Dtos;
using GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels;
using GolbonWebRoad.Web.Areas.Admin.Models.Users;

namespace GolbonWebRoad.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Category

            CreateMap<ProductViewModel, CreateProductCommand>();
            CreateMap<CreateProductViewModel, CreateProductCommand>();
            CreateMap<EditProductViewModel, UpdateProductCommand>();
            CreateMap<ProductDto, DeleteProductViewModel>();
            CreateMap<ProductDto, EditProductViewModel>();
            CreateMap<EditProductDto, UpdateProductCommand>();
            CreateMap<EditProductDto, EditProductViewModel>();
            #endregion


            #region Category 
            CreateMap<CategoryDto, CategorySummaryDto>().ReverseMap();
            CreateMap<CreateCategoryViewModel, CreateCategoryCommand>();
            CreateMap<CategoryDto, EditCategoryViewModel>();
            CreateMap<EditCategoryViewModel, UpdateCategoryCommand>().ReverseMap();
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
