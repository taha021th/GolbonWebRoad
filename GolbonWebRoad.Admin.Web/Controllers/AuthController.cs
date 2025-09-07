using GolbonWebRoad.Admin.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt; // این using را اضافه کنید
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        // اگر کاربر قبلا لاگین کرده، به صفحه اصلی هدایت شود
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClientFactory.CreateClient();
        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        var requestBody = new StringContent(
            JsonSerializer.Serialize(new { model.UserName, model.Password }),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync($"{apiBaseUrl}/api/auth/login", requestBody);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenData = JsonDocument.Parse(responseBody);
            var token = tokenData.RootElement.GetProperty("token").GetString();

            // --- شروع تغییرات ---
            // توکن را می‌خوانیم تا Claim ها را از داخل آن استخراج کنیم
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var claimsFromToken = jwtSecurityToken.Claims.ToList();

            // توکن را هم به عنوان یک Claim جداگانه ذخیره می‌کنیم تا در درخواست‌های بعدی به API از آن استفاده کنیم
            claimsFromToken.Add(new Claim("AccessToken", token));

            var claimsIdentity = new ClaimsIdentity(
                claimsFromToken,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, // مشخص می‌کنیم کدام Claim برای نام کاربری است
                ClaimTypes.Role   // مشخص می‌کنیم کدام Claim برای نقش است
            );
            // --- پایان تغییرات ---

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور اشتباه است.");
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }
}