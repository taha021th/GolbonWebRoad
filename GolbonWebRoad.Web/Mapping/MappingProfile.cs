using AutoMapper;
using GolbonWebRoad.Application.Dtos.Users;
using GolbonWebRoad.Application.Features.Brands.Commands;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Users.Commands;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Web.Areas.Admin.Models.Brands;
using GolbonWebRoad.Web.Areas.Admin.Models.Categories;
using GolbonWebRoad.Web.Areas.Admin.Models.Orders;
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
            CreateMap<Product, Areas.Admin.Models.Products.ViewModels.ProductViewModel>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Variants.Sum(v => v.StockQuantity)))
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.MainImageUrl)
                        ? src.MainImageUrl
                        : (src.Images != null && src.Images.Any()
                            ? (src.Images.Where(img => img.IsMainImage).Select(img => img.ImageUrl).FirstOrDefault()
                                ?? src.Images.Select(img => img.ImageUrl).FirstOrDefault())
                            : null)
                )
            );
            CreateMap<CreateProductViewModel, CreateProductCommand>();
            CreateMap<Product, EditProductViewModel>()
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.CurrentMainImageUrl, opt => opt.MapFrom(src => src.MainImageUrl));
            CreateMap<ProductVariant, VariantRowViewModel>()
                .ForMember(dest => dest.AttributeValueIds, opt => opt.MapFrom(src => src.AttributeValues.Select(av => av.Id).ToList()));
            CreateMap<EditProductViewModel, UpdateProductCommand>();
            CreateMap<ProductImage, ProductImageViewModel>();
            CreateMap<Product, DeleteProductViewModel>();
            #endregion

            #region Ui
            CreateMap<Product, Models.Products.ProductViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.MainImageUrl)
                        ? src.MainImageUrl
                        : (src.Images != null && src.Images.Any()
                            ? (src.Images.Where(i => i.IsMainImage).Select(i => i.ImageUrl).FirstOrDefault()
                                ?? src.Images.Select(i => i.ImageUrl).FirstOrDefault())
                            : null)));

            CreateMap<ProductImage, Models.Products.ProductImagesViewModel>();
            // Removed ProductColor mapping

            CreateMap<ProductVariant, GolbonWebRoad.Web.Models.Products.VariantDisplayViewModel>()
                .ForMember(dest => dest.AttributeValueIds, opt => opt.MapFrom(src => src.AttributeValues != null ? src.AttributeValues.Select(av => av.Id).ToList() : new List<int>()));

            CreateMap<ProductAttributeValue, GolbonWebRoad.Web.Models.Products.AttributeValueDisplayViewModel>();

            CreateMap<Product, Models.Products.ProductDetailViewModel>()
                .ForMember(dest => dest.ReviewsCount, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.MainImageUrl))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.AttributeGroups, opt => opt.MapFrom(src =>
                    (src.Variants != null)
                        ? src.Variants
                            .Where(v => v.AttributeValues != null)
                            .SelectMany(v => v.AttributeValues)
                            .GroupBy(av => av.AttributeId)
                            .Select(g => new GolbonWebRoad.Web.Models.Products.AttributeGroupDisplayViewModel
                            {
                                AttributeId = g.Key,
                                AttributeName = g.First().Attribute != null ? g.First().Attribute.Name : $"Attribute {g.Key}",
                                Values = g
                                    .GroupBy(av => av.Id)
                                    .Select(gr => new GolbonWebRoad.Web.Models.Products.AttributeValueDisplayViewModel
                                    {
                                        Id = gr.Key,
                                        Value = gr.First().Value
                                    })
                                    .ToList()
                            })
                            .OrderBy(gr => gr.AttributeName)
                            .ToList()
                        : new List<GolbonWebRoad.Web.Models.Products.AttributeGroupDisplayViewModel>()
                ));

            // Review mapping
            CreateMap<Review, Models.Products.ReviewViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "کاربر ناشناس"))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : ""));

            // Admin Review mapping
            CreateMap<Review, Areas.Admin.Models.Reviews.ReviewViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "کاربر ناشناس"))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : ""))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "محصول حذف شده"))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src =>
                    src.Product != null && src.Product.Images != null && src.Product.Images.Any()
                        ? src.Product.Images.Where(i => i.IsMainImage).Select(i => i.ImageUrl).FirstOrDefault()
                            ?? src.Product.Images.Select(i => i.ImageUrl).FirstOrDefault()
                        : null));

            // Cart view models
            CreateMap<Product, GolbonWebRoad.Web.Models.Cart.ProductCartViewModel>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.MainImageUrl)
                        ? src.MainImageUrl
                        : (src.Images != null && src.Images.Any()
                            ? (src.Images.Where(i => i.IsMainImage).Select(i => i.ImageUrl).FirstOrDefault() ?? src.Images.Select(i => i.ImageUrl).FirstOrDefault())
                            : null)))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));



            CreateMap<GolbonWebRoad.Application.Dtos.Products.ProductDto, GolbonWebRoad.Web.Models.Cart.ProductCartViewModel>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    src.Images != null && src.Images.Any()
                        ? (src.Images.Where(i => i.IsMainImage).Select(i => i.ImageUrl).FirstOrDefault() ?? src.Images.Select(i => i.ImageUrl).FirstOrDefault())
                        : null))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<GolbonWebRoad.Application.Dtos.CartItems.CartItemDto, GolbonWebRoad.Web.Models.Cart.CartItemViewModel>();

            // 2. Map Category/Brand DTOs to simple ViewModels for filter lists
            CreateMap<Category, Models.Products.CategoryViewModel>();
            CreateMap<Brand, Models.Products.BrandViewModel>();

            // 3. **FIX**: Create a generic map for PagedResult classes
            // This tells AutoMapper how to convert PagedResult from the Domain/Infrastructure layer
            // to the PagedResult (or PagedResultViewModel) class defined in the Web layer.
            CreateMap(typeof(GolbonWebRoad.Domain.Interfaces.Repositories.PagedResult<>), typeof(GolbonWebRoad.Web.Models.Products.PagedResult<>));

            #endregion
            #endregion


            #region Category 
            #region admin
            CreateMap<Category, GolbonWebRoad.Web.Areas.Admin.Models.Categories.CategoryViewModel>();
            CreateMap<CreateCategoryViewModel, CreateCategoryCommand>();
            CreateMap<Category, EditCategoryViewModel>()
                .ForMember(dest => dest.ExistingImage, opt => opt.MapFrom(src => src.ImageUrl));
            CreateMap<EditCategoryViewModel, UpdateCategoryCommand>();

            #endregion

            #region Ui

            #endregion
            #endregion


            #region Brand
            #region admin
            CreateMap<Brand, GolbonWebRoad.Web.Areas.Admin.Models.Brands.BrandViewModel>();
            CreateMap<Brand, EditBrandViewModel>();
            CreateMap<CreateBrandViewModel, CreateBrandCommand>();
            CreateMap<EditBrandViewModel, UpdateBrandCommand>();
            CreateMap<Brand, DeleteBrandViewModel>();
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

            #region Order
            CreateMap<Order, OrderIndexViewModel>()
                // Gets the user's name from the related IdentityUser entity
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("yyyy/MM/dd HH:mm")));

            // Mapping for Order detail view
            CreateMap<Order, OrderDetailViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("yyyy/MM/dd HH:mm")))
                .ForMember(dest => dest.AddressFullName, opt => opt.MapFrom(src => src.Address != null ? src.Address.FullName : null))
                .ForMember(dest => dest.AddressPhone, opt => opt.MapFrom(src => src.Address != null ? src.Address.Phone : null))
                .ForMember(dest => dest.AddressLine, opt => opt.MapFrom(src => src.Address != null ? src.Address.AddressLine : null))
                .ForMember(dest => dest.AddressCity, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : null))
                .ForMember(dest => dest.AddressPostalCode, opt => opt.MapFrom(src => src.Address != null ? src.Address.PostalCode : null));

            // Mapping for order items
            CreateMap<OrderItem, OrderItemViewModel>()
                // Gets the product name from the related Product entity
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
                .ForMember(dest => dest.VariantId, opt => opt.MapFrom(src => src.ProductVariantId))
                .ForMember(dest => dest.VariantAttributes, opt => opt.MapFrom(src =>
                    src.ProductVariant.AttributeValues != null && src.ProductVariant.AttributeValues.Any()
                        ? string.Join(" / ", src.ProductVariant.AttributeValues.Select(av => (av.Attribute != null ? av.Attribute.Name + ": " : "") + av.Value))
                        : string.Empty));
            #endregion



        }
    }
}
