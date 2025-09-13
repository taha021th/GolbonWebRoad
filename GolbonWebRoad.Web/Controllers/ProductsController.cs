using AutoMapper;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, string? sortOrder)
        {

            var products = await _mediator.Send(new GetProductsQuery { SearchTerm=searchTerm, CategoryId=categoryId, SortOrder=sortOrder });
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var viewModel = new ProductViewModel
            {
                Products=_mapper.Map<IEnumerable<ProductDto>>(products),
                Categories=_mapper.Map<IEnumerable<CategoryDto>>(categories)

            };
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentCategory"]=categoryId;
            ViewData["CurrentSort"]=sortOrder;
            return View(viewModel);
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id==0)
            {
                return NotFound();
            }

            var product = await _mediator.Send(new GetProductByIdQuery { Id=id });
            if (product==null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
