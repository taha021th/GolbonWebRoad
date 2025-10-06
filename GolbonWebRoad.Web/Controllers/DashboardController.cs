using GolbonWebRoad.Application.Features.Orders.Queries;
using GolbonWebRoad.Application.Features.Users.Queries;
using GolbonWebRoad.Web.Models;
using GolbonWebRoad.Web.Models.Addresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;
        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string? open = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _mediator.Send(new GetOrdersByUserIdQuery { UserId = userId });
            var addresses = await _mediator.Send(new GetUserAddressesQuery { UserId = userId });

            var vm = new DashboardViewModel
            {
                Orders = orders,
                Addresses = addresses.Select(a => new AddressItemViewModel
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    AddressLine = a.AddressLine,
                    City = a.City,
                    PostalCode = a.PostalCode,
                    IsDefault = a.IsDefault
                }).ToList(),

            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _mediator.Send(new GetOrderByIdQuery { Id = id });
            if (order == null || order.UserId != userId)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}


