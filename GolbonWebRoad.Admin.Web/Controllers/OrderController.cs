using GolbonWebRoad.Admin.Web.Dtos;
using GolbonWebRoad.Admin.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[Authorize(Roles = "Admin")]
public class OrderController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OrderController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient GetApiClient() => _httpClientFactory.CreateClient("ApiClient");

    // GET: /Order
    public async Task<IActionResult> Index()
    {
        var client = GetApiClient();
        var response = await client.GetAsync("/api/orders");

        if (response.IsSuccessStatusCode)
        {
            var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
            return View(orders);
        }

        ViewBag.ErrorMessage = "خطا در دریافت لیست سفارشات.";
        return View(new List<OrderDto>());
    }

    // GET: /Order/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var client = GetApiClient();
        var response = await client.GetAsync($"/api/orders/{id}");

        if (response.IsSuccessStatusCode)
        {
            var order = await response.Content.ReadFromJsonAsync<OrderDto>();

            // مدل ویو برای فرم آپدیت را می‌سازیم
            var viewModel = new UpdateOrderStatusViewModel
            {
                OrderId = order.Id,
                CurrentStatus = order.OrderStatus
            };

            // مدل اصلی و مدل ویو را به ویو پاس می‌دهیم
            ViewBag.UpdateStatusViewModel = viewModel;
            return View(order);
        }

        return NotFound();
    }

    // POST: /Order/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(UpdateOrderStatusViewModel model)
    {
        if (string.IsNullOrEmpty(model.NewStatus))
        {
            // اگر وضعیتی انتخاب نشده بود، به صفحه جزئیات برگردان
            TempData["StatusError"] = "لطفاً یک وضعیت جدید انتخاب کنید.";
            return RedirectToAction("Details", new { id = model.OrderId });
        }

        var client = GetApiClient();
        var requestBody = new
        {
            orderId = model.OrderId,
            orderStatus = model.NewStatus
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/orders/UpdateStatus", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["StatusSuccess"] = "وضعیت سفارش با موفقیت به‌روزرسانی شد.";
        }
        else
        {
            TempData["StatusError"] = "خطا در به‌روزرسانی وضعیت سفارش.";
        }

        return RedirectToAction("Details", new { id = model.OrderId });
    }
}