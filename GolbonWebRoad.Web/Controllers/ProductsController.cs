using AutoMapper;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Application.Features.Reviews.Commands;
using GolbonWebRoad.Application.Features.Reviews.Queries;
using GolbonWebRoad.Web.Models.Products;
using GolbonWebRoad.Web.Services.Seo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ISeoSettingsService _seo;
        public ProductsController(IMediator mediator, IMapper mapper, ISeoSettingsService seo)
        {
            _mediator=mediator;
            _mapper=mapper;
            _seo = seo;
        }
        public async Task<IActionResult> Index(int? categoryId, int? brandId, string searchTerm, string sortOrder, int page = 1)
        {
            var data = await _mediator.Send(new GetProductsPageDataQuery
            {
                CategoryId = categoryId,
                BrandId = brandId,
                SearchTerm = searchTerm,
                SortOrder = sortOrder,
                PageNumber = page,
                PageSize = 6
            });

            var viewModel = new ProductIndexViewModel
            {
                Products = _mapper.Map<PagedResult<ProductViewModel>>(data.Products),
                Categories = _mapper.Map<List<CategoryViewModel>>(data.Categories),
                Brands = _mapper.Map<List<BrandViewModel>>(data.Brands),
                CurrentCategoryId = categoryId,
                CurrentBrandId = brandId,
                SearchTerm = searchTerm,
                CurrentSortOrder = sortOrder
            };

            // Dynamic SEO Meta Tags Generation
            if (categoryId.HasValue)
            {
                var category = viewModel.Categories.FirstOrDefault(c => c.Id == categoryId.Value);
                if (category != null)
                {
                    viewModel.MetaTitle = $"خرید و قیمت {category.Name} | فروشگاه Tecture";
                    viewModel.MetaDescription = $"آخرین مدل‌های {category.Name} را با بهترین قیمت و کیفیت از فروشگاه آنلاین Tecture بررسی و خریداری کنید.";
                }
            }
            else if (brandId.HasValue)
            {
                var brand = viewModel.Brands.FirstOrDefault(b => b.Id == brandId.Value);
                if (brand != null)
                {
                    viewModel.MetaTitle = $"محصولات برند {brand.Name} | فروشگاه Tecture";
                    viewModel.MetaDescription = $"جدیدترین محصولات برند {brand.Name} را با ضمانت اصالت کالا از Tecture خریداری کنید.";
                }
            }
            else if (!string.IsNullOrEmpty(searchTerm))
            {
                viewModel.MetaTitle = $"نتایج جستجو برای '{searchTerm}'";
                viewModel.MetaDescription = $"نتایج جستجوی محصولات برای عبارت '{searchTerm}' در فروشگاه Tecture.";
            }
            else
            {
                viewModel.MetaTitle = "همه محصولات | فروشگاه Tecture";
                viewModel.MetaDescription = "مجموعه کامل محصولات و مبلمان فروشگاه آنلاین دکوراسیون داخلی Tecture را مشاهده کنید.";
            }

            // اگر صفحه لیست محصولات با فیلتر (دسته/برند/جستجو) باز شود، برای جلوگیری از ایندکس شدن نتایج فیلترشده، تگ noindex اعمال می‌کنیم
            var settings = await _seo.GetAsync();
            if (settings.NoindexOnFilteredLists && (categoryId.HasValue || brandId.HasValue || !string.IsNullOrWhiteSpace(searchTerm)))
            {
                ViewBag.Noindex = true;
            }

            return View(viewModel);
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }


            var productEntity = await _mediator.Send(new GetProductByIdQuery { Id = id, JoinCategory = true, JoinImages = true, JoinBrand = true, JoinReviews = true });
            if (productEntity == null)
            {
                return NotFound();
            }
            var reviews = await _mediator.Send(new GetReviewsByProductIdQuery { ProductId = id, JoinUser = true });
            // ۵. مپ کردن انتیتی اصلی به ViewModel
            var viewModel = _mapper.Map<ProductDetailViewModel>(productEntity);

            // پر کردن فیلدهای سئو محصول: اگر MetaTitle/Description تنظیم نشده باشد، از قالب‌های پیش‌فرض استفاده می‌کنیم
            var settings = await _seo.GetAsync();
            viewModel.MetaTitle = !string.IsNullOrWhiteSpace(productEntity.MetaTitle)
                ? productEntity.MetaTitle
                : (settings.DefaultMetaTitleTemplate ?? "{name}").Replace("{name}", productEntity.Name ?? "");
            viewModel.MetaDescription = !string.IsNullOrWhiteSpace(productEntity.MetaDescription)
                ? productEntity.MetaDescription
                : (settings.DefaultMetaDescriptionTemplate ?? "خرید {name}").Replace("{name}", productEntity.Name ?? "");
            if (settings.AutoCanonicalEnabled && string.IsNullOrWhiteSpace(productEntity.CanonicalUrl))
            {
                var req = HttpContext.Request;
                viewModel.CanonicalUrl = $"{req.Scheme}://{req.Host}/Products/Detail/{productEntity.Id}";
            }
            else
            {
                viewModel.CanonicalUrl = productEntity.CanonicalUrl;
            }
            viewModel.MainImageAltText = productEntity.MainImageAltText;

            // ۶. پردازش و آماده‌سازی ViewModel با اطلاعات دریافتی از کوئری‌ها
            viewModel.Reviews = _mapper.Map<List<ReviewViewModel>>(reviews);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(ReviewFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "لطفاً تمام فیلدها را به درستی پر کنید.";
                return RedirectToAction("Detail", new { id = model.ProductId });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Error"] = "لطفاً ابتدا وارد حساب کاربری خود شوید.";
                    return RedirectToAction("Detail", new { id = model.ProductId });
                }

                await _mediator.Send(new CreateReviewCommand
                {
                    ProductId = model.ProductId,
                    ReviewText = model.ReviewText,
                    Rating = model.Rating,
                    UserId = userId
                });

                TempData["Success"] = "نظر شما با موفقیت ثبت شد و پس از تایید نمایش داده خواهد شد.";
                return RedirectToAction("Detail", new { id = model.ProductId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "خطایی در ثبت نظر رخ داد. لطفاً دوباره تلاش کنید.";
                return RedirectToAction("Detail", new { id = model.ProductId });
            }
        }
    }
}
