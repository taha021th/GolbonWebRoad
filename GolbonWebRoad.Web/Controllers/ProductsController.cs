using AutoMapper;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Application.Features.Reviews.Commands;
using GolbonWebRoad.Application.Features.Reviews.Queries;
using GolbonWebRoad.Web.Models.Products;
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
        public ProductsController(IMediator mediator, IMapper mapper)
        {
            _mediator=mediator;
            _mapper=mapper;
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
