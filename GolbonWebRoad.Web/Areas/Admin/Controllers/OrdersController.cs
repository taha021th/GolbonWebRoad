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

        // سرویس لجستیک از طریق سازنده تزریق نمی‌شود چون فقط در یک متد استفاده می‌شود
        // و می‌توان آن را مستقیم در متد از Service Locator گرفت یا یک کامند جدا برای آن ساخت.
        // روش تمیزتر استفاده از کامند است که ما همین کار را کرده‌ایم.
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
            var order = await _mediator.Send(new GetOrderByIdQuery { Id = id });
            if (order == null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<OrderDetailViewModel>(order);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (id == 0 || string.IsNullOrEmpty(status))
            {
                return BadRequest();
            }
            await _mediator.Send(new UpdateOrderStatusCommand { OrderId = id, OrderStatus = status });
            TempData["SuccessMessage"] = "وضعیت سفارش با موفقیت به‌روزرسانی شد.";
            return RedirectToAction("Detail", new { id = id });
        }

        // ==========================================================
        // === اکشن جدید برای ثبت مرسوله و دریافت کد رهگیری ===
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShipment(int orderId)
        {
            if (orderId == 0)
            {
                return BadRequest();
            }

            try
            {
                var command = new CreateShipmentCommand { OrderId = orderId };
                var trackingNumber = await _mediator.Send(command);

                TempData["SuccessMessage"] = $"مرسوله با موفقیت ثبت شد. کد رهگیری: {trackingNumber}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطا در ثبت مرسوله: {ex.Message}";
            }

            return RedirectToAction("Detail", new { id = orderId });
        }
    }
}
