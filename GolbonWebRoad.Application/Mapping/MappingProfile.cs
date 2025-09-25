using AutoMapper;
using GolbonWebRoad.Application.Dtos.Brands;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Dtos.Colors;
using GolbonWebRoad.Application.Dtos.Logs;
using GolbonWebRoad.Application.Dtos.Orders;
using GolbonWebRoad.Application.Dtos.ProductColors;
using GolbonWebRoad.Application.Dtos.ProductImages;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Application.Features.Brands.Commands;
using GolbonWebRoad.Application.Features.CartItems.Commands;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Reviews.Commands;
using GolbonWebRoad.Domain.Entities;

namespace GolbonWebRoad.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region CartItems
            // --- نگاشت‌های مربوط به سبد خرید ---

            // از DTO ورودی کنترلر به کامند داخلی اپلیکیشن
            // استفاده: در اکشن AddToCart در CartItemController
            CreateMap<AddToCartRequestDto, AddToCartCommand>();

            // از DTO ورودی کنترلر به کامند داخلی اپلیکیشن
            // استفاده: در اکشن UpdateCartItem در CartItemController
            CreateMap<UpdateCartItemRequestDto, UpdateCartItemCommand>();

            // از کامند داخلی به موجودیت دامنه برای ذخیره در دیتابیس
            // استفاده: در AddToCartCommandHandler
            CreateMap<AddToCartCommand, CartItem>();

            // از موجودیت دامنه به DTO خروجی برای نمایش به کاربر
            // استفاده: در GetCartQueryHandler برای نمایش آیتم‌های سبد خرید
            CreateMap<CartItem, CartItemDto>();
            CreateMap<CartItem, CartItemSummaryDto>();
            #endregion

            #region Reviews
            CreateMap<CreateReviewCommand, Review>();
            #endregion

            #region Categories
            // --- نگاشت‌های مربوط به دسته‌بندی‌ها ---

            // از موجودیت دامنه به DTO کامل برای نمایش جزئیات
            // استفاده: در GetCategoryByIdQueryHandler و GetCategoriesQueryHandler
            CreateMap<Category, CategoryDto>();

            // از موجودیت دامنه به DTO خلاصه‌شده برای جلوگیری از ارسال داده اضافی
            // استفاده: به عنوان پراپرتی در ProductDto برای نمایش نام دسته‌بندی محصول
            CreateMap<Category, CategorySummaryDto>();
            CreateMap<GetCategoriesQuery, CategoryDto>();

            // از DTO ورودی کنترلر به کامند داخلی اپلیکیشن
            // استفاده: در اکشن Create در CategoryController
            CreateMap<CreateCategoryRequestDto, CreateCategoryCommand>();

            // از DTO ورودی کنترلر به کامند داخلی اپلیکیشن
            // استفاده: در اکشن Update در CategoryController
            CreateMap<UpdateCategoryRequestDto, UpdateCategoryCommand>();

            // از کامند داخلی به موجودیت دامنه برای ذخیره در دیتابیس
            // استفاده: در CreateCategoryCommandHandler
            CreateMap<CreateCategoryCommand, Category>();

            // از کامند داخلی به موجودیت دامنه برای به‌روزرسانی در دیتابیس
            // استفاده: در UpdateCategoryCommandHandler
            CreateMap<UpdateCategoryCommand, Category>();
            #endregion

            #region Brands
            CreateMap<Brand, BrandDto>();
            CreateMap<CreateBrandCommand, Brand>();
            CreateMap<UpdateBrandCommand, Brand>();
            #endregion


            #region Products
            // --- نگاشت‌های مربوط به محصولات ---

            // نگاشت دوطرفه بین موجودیت محصول و DTO کامل آن
            // استفاده (Product -> ProductDto): در تمام کوئری‌های محصولات برای نمایش به کاربر
            // استفاده (ProductDto -> Product): در سناریوهایی که ممکن است نیاز به تبدیل برعکس باشد
            CreateMap<Product, ProductDto>().ReverseMap();
            //برای تبدیل پروداکت به ProductSummaryDto در CategoryDto
            CreateMap<Product, ProductSummaryDto>();
            CreateMap<Product, ProductAdminSummaryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "N/A"))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : "N/A"))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsMainImage).ImageUrl));

            // از DTO ورودی کنترلر به کامند داخلی اپلیکیشن، با نادیده گرفتن فایل
            // استفاده: در اکشن Create در ProductsController
            CreateMap<CreateProductRequestDto, CreateProductCommand>()
                .ForMember(des => des.Images, opt => opt.Ignore());


            CreateMap<CreateProductCommand, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());




            //CreateMap<Product, ProductDto>()
            //    .ForMember(dest => dest.ImageUrl,
            //               opt => opt.MapFrom(src =>
            //                   src.Images != null && src.Images.Any()
            //                   ? src.Images.FirstOrDefault(i => i.IsMainImage)?.ImageUrl
            //                   : null));
            CreateMap<UpdateProductCommand, Product>();
            CreateMap<ProductImage, ProductImageDto>();


            #endregion


            #region ProductColor

            CreateMap<ProductColor, ProductColorDto>();
            #endregion
            #region Color
            CreateMap<Color, ColorDto>();
            #endregion

            #region Orders
            // --- نگاشت‌های مربوط به سفارشات ---

            // نگاشت دوطرفه بین موجودیت سفارش و DTO کامل آن
            // استفاده (Order -> OrderDto): در تمام کوئری‌های سفارشات برای نمایش به کاربر
            CreateMap<Order, OrderDto>().ReverseMap();

            // از موجودیت آیتم سفارش به DTO آن
            // استفاده: به عنوان پراپرتی در OrderDto برای نمایش جزئیات سفارش
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<OrderItem, OrderItemSummaryDto>();

            // از DTO ورودی کنترلر به کامند داخلی اپلیکیشن
            // استفاده: در اکشن Create در OrdersController
            CreateMap<CreateOrderRequestDto, CreateOrderCommand>();

            // از DTO ورودی کنترلر به کامند داخلی اپلیکیشن
            // استفاده: در اکشن UpdateStatus در OrdersController
            CreateMap<UpdateOrderStatusRequestDto, UpdateOrderStatusCommand>();
            #endregion

            #region Logs
            CreateMap<Log, LogDto>();
            #endregion
        }
    }
}