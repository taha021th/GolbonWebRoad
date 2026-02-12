using AutoMapper;
using GolbonWebRoad.Web.Models;
using GolbonWebRoad.Web.Models.Categories;
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
            var data = await _mediator.Send(new GolbonWebRoad.Application.Features.HomePage.Queries.GetHomePageDataQuery());

            var viewModel = new HomeProductViewModel
            {
                Products = _mapper.Map<List<ProductViewModel>>(data.Products),
                ProductIsFeatured = _mapper.Map<ProductViewModel>(data.ProductIsFeatured),
                Categories = _mapper.Map<List<CategoryViewModel>>(data.Categories),
                Blogs = _mapper.Map<List<GolbonWebRoad.Web.Models.Blogs.BlogSummaryViewModel>>(data.Blogs),
                Reviews=_mapper.Map<List<ReviewViewModel>>(data.Reviews),
            };

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
