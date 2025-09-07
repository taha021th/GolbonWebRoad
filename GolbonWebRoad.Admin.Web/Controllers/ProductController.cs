using GolbonWebRoad.Admin.Web.Dtos;
using GolbonWebRoad.Admin.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    // ۱. سرویس IConfiguration را اضافه می‌کنیم
    private readonly IConfiguration _configuration;


    // ۲. کانستراکتور را برای دریافت IConfiguration به‌روز می‌کنیم
    public ProductController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;

    }

    private HttpClient GetApiClient() => _httpClientFactory.CreateClient("ApiClient");

    // متد Index بدون تغییر است
    public async Task<IActionResult> Index()
    {
        var client = GetApiClient();

        var response = await client.GetAsync("/api/products");

        //if (response.IsSuccessStatusCode)
        //{
        //    var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        //    return View(products);
        //}

        return View(new List<ProductDto>());
    }

    private async Task<List<SelectListItem>> GetCategoriesAsync()
    {
        var client = GetApiClient();
        var response = await client.GetAsync("/api/category");

        if (response.IsSuccessStatusCode)
        {
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
            return categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
        }
        return new List<SelectListItem>();
    }

    // متد Create (هم GET و هم POST) بدون تغییر است
    public async Task<IActionResult> Create()
    {
        var viewModel = new ProductViewModel
        {
            Categories = await GetCategoriesAsync()
        };
        return View(viewModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoriesAsync();
            return View(model);
        }

        var client = GetApiClient();
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(model.Name), nameof(model.Name));
        content.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
        content.Add(new StringContent(model.Price.ToString()), nameof(model.Price));
        content.Add(new StringContent(model.Stock.ToString()), nameof(model.Stock));
        content.Add(new StringContent(model.CategoryId.ToString()), nameof(model.CategoryId));

        if (model.ImageFile != null)
        {
            var streamContent = new StreamContent(model.ImageFile.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
            content.Add(streamContent, nameof(model.ImageFile), model.ImageFile.FileName);
        }

        var response = await client.PostAsync("/api/products", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "خطا در ایجاد محصول.");
        model.Categories = await GetCategoriesAsync();
        return View(model);
    }

    // GET: /Product/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var client = GetApiClient();
        var response = await client.GetAsync($"/api/products/{id}");

        if (!response.IsSuccessStatusCode) return NotFound();

        var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
        if (product == null) return NotFound();

        product.Categories = await GetCategoriesAsync();

        // --- شروع تغییرات ---
        // ۳. آدرس پایه API را از کانفیگ می‌خوانیم و URL تصویر را می‌سازیم
        if (!string.IsNullOrEmpty(product.ExistingImageUrl))
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            product.ExistingImageUrl = $"{apiBaseUrl}{product.ExistingImageUrl}";
        }
        // --- پایان تغییرات ---

        return View(product);
    }

    // متد Edit (POST) بدون تغییر است
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel model)
    {
        if (id != model.Id) return BadRequest();

        ModelState.Remove(nameof(model.ImageFile));

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoriesAsync();
            return View(model);
        }

        var client = GetApiClient();
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(model.Id.ToString()), nameof(model.Id));
        content.Add(new StringContent(model.Name), nameof(model.Name));
        content.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
        content.Add(new StringContent(model.Price.ToString()), nameof(model.Price));
        content.Add(new StringContent(model.Stock.ToString()), nameof(model.Stock));
        content.Add(new StringContent(model.CategoryId.ToString()), nameof(model.CategoryId));

        if (model.ImageFile != null)
        {
            var streamContent = new StreamContent(model.ImageFile.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
            content.Add(streamContent, nameof(model.ImageFile), model.ImageFile.FileName);
        }

        var response = await client.PutAsync($"/api/products/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "خطا در ویرایش محصول.");
        model.Categories = await GetCategoriesAsync();
        return View(model);
    }

    // شما باید متدهای Delete را خودتان اضافه کنید (مشابه CategoryController)
}