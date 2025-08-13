using GolbonWebRoad.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator=mediator;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _mediator.Send(new GetProductsQuery());
            return View(products);
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
