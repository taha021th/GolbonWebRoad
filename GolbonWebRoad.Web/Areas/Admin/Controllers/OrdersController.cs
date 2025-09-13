using AutoMapper;
using GolbonWebRoad.Application.Features.Orders.Commands;
using GolbonWebRoad.Application.Features.Orders.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.Orders;
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
        private readonly IMapper _mapper;
        public OrdersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _mediator.Send(new GetAllOrdersQuery());
            var viewModel = _mapper.Map<IEnumerable<OrderIndexViewModel>>(orders);
            return View(viewModel);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery { Id=id });
            if (order==null)
            {

                return NotFound();
            }
            var viewModel = _mapper.Map<OrderDetailViewModel>(order);
            return View(viewModel);

        }
        [HttpPost]
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
