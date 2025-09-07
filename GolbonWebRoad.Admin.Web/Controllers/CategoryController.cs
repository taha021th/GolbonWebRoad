using GolbonWebRoad.Admin.Web.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CategoryController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient GetApiClient() => _httpClientFactory.CreateClient("ApiClient");

    // GET: /Category
    public async Task<IActionResult> Index()
    {
        var client = GetApiClient();
        var response = await client.GetAsync("/api/category");

        if (response.IsSuccessStatusCode)
        {
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
            return View(categories);
        }

        // Handle error appropriately
        ViewBag.ErrorMessage = "خطا در دریافت اطلاعات از سرور.";
        return View(new List<CategoryDto>());
    }

    // GET: /Category/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = GetApiClient();
        var content = new StringContent(JsonSerializer.Serialize(new { model.Name }), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/category", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "خطا در ایجاد دسته‌بندی جدید.");
        return View(model);
    }

    // GET: /Category/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var client = GetApiClient();
        var response = await client.GetAsync($"/api/category/{id}");

        if (response.IsSuccessStatusCode)
        {
            var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return View(category);
        }

        return NotFound();
    }

    // POST: /Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryDto model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var client = GetApiClient();
            var content = new StringContent(JsonSerializer.Serialize(new { model.Name }), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/category/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "خطا در ویرایش دسته‌بندی.");
        }
        return View(model);
    }

    // GET: /Category/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var client = GetApiClient();
        var response = await client.GetAsync($"/api/category/{id}");

        if (response.IsSuccessStatusCode)
        {
            var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return View(category);
        }

        return NotFound();
    }

    // POST: /Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = GetApiClient();
        await client.DeleteAsync($"/api/category/{id}");
        return RedirectToAction(nameof(Index));
    }
}