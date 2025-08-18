using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _mediator.Send(new GetAllOrdersQuery());
            return View(orders);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery { Id=id });
            if (order==null)
            {

                return NotFound();
            }
            return View(order);

        }
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {

            if (id==0 || string.IsNullOrEmpty(status))
            {
                return BadRequest();
            }
            await _mediator.Send(new UpdateOrderStatusCommand { OrderId=id, OrderStatus=status });
            return RedirectToAction("Detail", new { id = id });
        }
    }
}
