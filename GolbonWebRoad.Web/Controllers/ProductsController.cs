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
        public async Task<IActionResult> Details(int id)
        {

            var product = await _mediator.Send(new GetProductByIdQuery { Id=id });
            if (product==null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
