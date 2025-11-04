using AutoMapper;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Web.Models.Categories;
using GolbonWebRoad.Web.Models.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public CategoriesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> Index()
        {
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var viewModel = _mapper.Map<List<CategorySummaryViewModel>>(categories);
            return View(viewModel);
        }

        [HttpGet]
        [Route("category/{id:int}/{slug?}", Name = "Category")]
        [Route("Categories/Detail/{id?}")] // backward-compat for conventional route
        public async Task<IActionResult> Detail(int id, string? sortOrder, int page = 1)
        {
            if (id <= 0) return NotFound();

            var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
            if (category == null) return NotFound();

            var data = await _mediator.Send(new GetProductsPageDataQuery
            {
                CategoryId = id,
                SortOrder = sortOrder,
                PageNumber = page,
                PageSize = 12
            });

            var viewModel = new ProductIndexViewModel
            {
                Products = _mapper.Map<PagedResult<ProductViewModel>>(data.Products),
                Categories = _mapper.Map<List<GolbonWebRoad.Web.Models.Products.CategoryViewModel>>(data.Categories),
                Brands = _mapper.Map<List<GolbonWebRoad.Web.Models.Products.BrandViewModel>>(data.Brands),
                CurrentCategoryId = id,
                CurrentBrandId = null,
                SearchTerm = null,
                CurrentSortOrder = sortOrder,
                MetaTitle = $"محصولات دسته {category.Name}",
                MetaDescription = $"خرید و مشاهده محصولات دسته {category.Name}"
            };
            ViewBag.Noindex = true; // فهرست فیلتر شده
            ViewBag.HidePagination = false;

            return View(viewModel);
        }
    }
}
