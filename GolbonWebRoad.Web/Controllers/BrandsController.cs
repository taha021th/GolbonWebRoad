using AutoMapper;
using GolbonWebRoad.Application.Features.Brands.Queries;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Web.Models.Brands;
using GolbonWebRoad.Web.Models.Categories;
using GolbonWebRoad.Web.Models.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Controllers
{
    public class BrandsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public BrandsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: /brands
        [HttpGet]
        [Route("brands")]
        public async Task<IActionResult> Index()
        {
            var brands = await _mediator.Send(new GetBrandsQuery());
            var viewModel = _mapper.Map<List<BrandSummaryViewModel>>(brands);
            return View(viewModel);
        }

        // GET: /brand/5/سامسونگ?page=2&sortOrder=price_desc
        [HttpGet]
        [Route("brand/{id:int}/{slug?}", Name = "BrandDetail")]
        [Route("Brands/Detail/{id?}")] // سازگاری با روت قدیمی
        public async Task<IActionResult> Detail(int id, string? slug, string? sortOrder, int page = 1)
        {
            if (id <= 0) return NotFound();

            var brand = await _mediator.Send(new GetBrandByIdQuery { Id = id });
            if (brand == null) return NotFound();

            const int pageSize = 12;
            if (page < 1) page = 1;

            var productsData = await _mediator.Send(new GetProductsPageDataQuery
            {
                BrandId = id,
                SortOrder = sortOrder,
                PageNumber = page,
                PageSize = pageSize
            });

            var viewModel = new BrandProductsIndexViewModel
            {
                Products = _mapper.Map<PagedResult<ProductViewModel>>(productsData.Products),
                Categories = _mapper.Map<List<CategoryViewModel>>(productsData.Categories),
                Brands = _mapper.Map<BrandViewModel>(brand),
                CurrentCategoryId = null,
                CurrentBrandId = id,
                SearchTerm = null,
                CurrentSortOrder = sortOrder ?? string.Empty,
                MetaTitle = $"محصولات برند {brand.Name} | فروشگاه گلبن",
                MetaDescription = $"خرید و مشاهده تمام محصولات برند {brand.Name} با قیمت مناسب و ارسال سریع."
            };

            // noindex فقط وقتی فیلتر فعال است (نه صرفاً صفحه‌بندی)
            ViewBag.Noindex = !string.IsNullOrWhiteSpace(sortOrder);
            ViewBag.HidePagination = false;

            // ریدایرکت به slug درست (سئو)
            if (!string.IsNullOrEmpty(slug) && !slug.Equals(Slugify(brand.Name), StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToRoutePermanent("BrandDetail", new
                {
                    id = id,
                    slug = Slugify(brand.Name),
                    sortOrder,
                    page
                });
            }

            return View(viewModel);
        }

        private static string Slugify(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return text.Replace(" ", "-")
                       .Replace("‌", "-")
                       .ToLowerInvariant()
                       .Pipe(s => System.Text.RegularExpressions.Regex.Replace(s, @"[^a-z0-9\-]", ""));
        }
    }

    // متد کمکی برای Pipe (اختیاری، اما تمیز)
    public static class StringExtensions
    {
        public static TResult Pipe<T, TResult>(this T input, Func<T, TResult> selector)
            => selector(input);
    }
}