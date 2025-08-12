using GolbonWebRoad.Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<IdentityUser> _userManager;
        public AccountController(IMediator mediator, UserManager<IdentityUser> userManager)
        {
            _mediator=mediator;
            _userManager=userManager;
        }
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var orders = await _mediator.Send(new GetOrdersByUserIdQuery { UserId=userId });
            return View(orders);
        }
    }
}
