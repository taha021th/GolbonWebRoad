using AutoMapper;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Web.Models;
using GolbonWebRoad.Web.Models.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GolbonWebRoad.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public HomeController(IMediator mediator, IMapper mapper)
        {
            _mediator=mediator;
            _mapper=mapper;
        }

        public async Task<IActionResult> Index()
        {
            var productsTask = await _mediator.Send(new GetProductsQuery { SortOrder="price_desc", JoinImages=true, JoinBrand=true, Count=8 });
            var productIsFeaturedTask = await _mediator.Send(new GetProductByIsFeaturedQuery()); ;
            var categoriesTask = await _mediator.Send(new GetCategoriesQuery { Take=9 });



            var viewModel = new HomeProductViewModel();
            viewModel.Products= _mapper.Map<List<ProductViewModel>>(productsTask);
            viewModel.ProductIsFeatured =_mapper.Map<ProductViewModel>(productIsFeaturedTask);
            viewModel.Categories=_mapper.Map<List<CategoryViewModel>>(categoriesTask);

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
