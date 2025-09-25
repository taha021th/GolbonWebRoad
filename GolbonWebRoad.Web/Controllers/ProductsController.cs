using AutoMapper;
using GolbonWebRoad.Application.Features.Brands.Queries;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Application.Features.Reviews.Queries;
using GolbonWebRoad.Application.Features.Reviews.Commands;
using GolbonWebRoad.Web.Models.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
            // 1. Fetch raw product entities using the new query
            var pagedProducts = await _mediator.Send(new GetPagedProductsQuery
            {
                CategoryId = categoryId,
                BrandId = brandId,
                SearchTerm = searchTerm,
                SortOrder=sortOrder,
                PageNumber = page,
                PageSize = 6
            });

            // 2. Fetch categories and brands for the filter sidebar
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var brands = await _mediator.Send(new GetBrandsQuery());

            // 3. Map everything to the final ViewModel here in the Controller
            var viewModel = new ProductIndexViewModel
            {
                // The mapping now happens on the paged result of entities
                Products = _mapper.Map<PagedResult<ProductViewModel>>(pagedProducts),
                Categories = _mapper.Map<List<CategoryViewModel>>(categories),
                Brands = _mapper.Map<List<BrandViewModel>>(brands),
                CurrentCategoryId = categoryId,
                CurrentBrandId = brandId,
                SearchTerm = searchTerm,
                CurrentSortOrder=sortOrder
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id==0)
            {
                return NotFound();
            }

            var product = await _mediator.Send(new GetProductByIdQuery
            {
                Id = id,
                JoinImages = true,
                JoinColors = true,
                JoinBrand = true,
                JoinCategory = true,
                JoinReviews = true
            });
            if (product==null)
            {
                return NotFound();
            }
            
            // Fetch approved reviews for this product
            var reviews = await _mediator.Send(new GetReviewsByProductIdQuery
            {
                ProductId = id,
                JoinUser = true
            });
            
            var viewModel = _mapper.Map<GolbonWebRoad.Web.Models.Products.ProductDetailViewModel>(product);

            // Map variants and attribute groups from repositories
            var variants = await _unitOfWork.ProductVariantRepository.GetByProductIdAsync(id);
            viewModel.Variants = variants.Select(v => new Models.Products.VariantDisplayViewModel
            {
                Id = v.Id,
                Sku = v.Sku,
                Price = v.Price,
                OldPrice = v.OldPrice,
                StockQuantity = v.StockQuantity,
                AttributeValueIds = v.SelectedAttributes?.Select(a => a.Id).ToList() ?? new List<int>()
            }).ToList();

            var attrs = await _unitOfWork.ProductAttributeRepository.GetAllAsync(1, int.MaxValue);
            var values = await _unitOfWork.ProductAttributeValueRepository.GetAllAsync(1, int.MaxValue);
            viewModel.AttributeGroups = values.Items
                .GroupBy(v => v.AttributeId)
                .Select(g => new Models.Products.AttributeGroupDisplayViewModel
                {
                    AttributeId = g.Key,
                    AttributeName = attrs.Items.FirstOrDefault(a => a.Id == g.Key)?.Name ?? $"Attribute {g.Key}",
                    Values = g.Select(v => new Models.Products.AttributeValueDisplayViewModel { Id = v.Id, Value = v.Value }).ToList()
                })
                .OrderBy(gr => gr.AttributeName)
                .ToList();
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
