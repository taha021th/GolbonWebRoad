#nullable disable

using GolbonWebRoad.Domain.Entities; // کلاس سفارشی کاربر که خودمان ساختیم را وارد می‌کنیم
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace GolbonWebRoad.Web.Areas.Identity.Pages.Account
{
    /// <summary>
    /// این کلاس PageModel نام دارد و منطق سمت سرور برای صفحه Register.cshtml را مدیریت می‌کند.
    /// تمام کارهایی که بعد از کلیک کاربر روی دکمه "ثبت نام" انجام می‌شود، در این کلاس نوشته شده است.
    /// </summary>
    public class RegisterModel : PageModel
    {
        // این بخش، سرویس‌های مورد نیاز برای مدیریت کاربران را تعریف می‌کند.
        // این سرویس‌ها به صورت خودکار توسط سیستم Dependency Injection به کلاس تزریق می‌شوند.

        private readonly SignInManager<ApplicationUser> _signInManager; // مسئول مدیریت ورود و خروج کاربران
        private readonly UserManager<ApplicationUser> _userManager;   // مسئول اصلی ساختن، حذف کردن و مدیریت کاربران
        private readonly IUserStore<ApplicationUser> _userStore;       // مسئول ذخیره و بازیابی اطلاعات کاربر از دیتابیس
        private readonly IUserEmailStore<ApplicationUser> _emailStore;  // مسئول مدیریت ایمیل کاربر
        private readonly ILogger<RegisterModel> _logger;              // برای ثبت وقایع و خطاها (لاگ کردن)
        private readonly IEmailSender _emailSender;                   // برای ارسال ایمیل (مثلا ایمیل تایید حساب)

        /// <summary>
        /// این سازنده (Constructor) کلاس است.
        /// وقتی یک نمونه از RegisterModel ساخته می‌شود، این متد فراخوانی شده و تمام سرویس‌های مورد نیاز را دریافت می‌کند.
        /// </summary>
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        /// این پراپرتی با اتریبیوت [BindProperty] مشخص شده است.
        /// این یعنی ASP.NET Core به صورت خودکار اطلاعاتی که از فرم ثبت‌نام ارسال می‌شود را در این پراپرتی قرار می‌دهد.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// آدرسی که کاربر پس از ثبت‌نام موفق باید به آنجا هدایت شود.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// لیستی از سرویس‌دهندگان خارجی برای ورود (مثل گوگل، فیسبوک و...)
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// این کلاس یک مدل داخلی است که فقط برای دریافت اطلاعات از فرم ثبت‌نام استفاده می‌شود.
        /// تمام فیلدهای فرم در این کلاس تعریف می‌شوند.
        /// </summary>
        public class InputModel
        {
            // فیلدهای سفارشی که خودمان اضافه کردیم
            [Required(ErrorMessage = "وارد کردن نام الزامی است.")]
            [Display(Name = "نام")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "وارد کردن نام خانوادگی الزامی است.")]
            [Display(Name = "نام خانوادگی")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "وارد کردن شماره تماس الزامی است.")]
            [Phone(ErrorMessage = "فرمت شماره تماس صحیح نیست.")]
            [Display(Name = "شماره تماس")]
            public string PhoneNumber { get; set; }

            // فیلدهای استاندارد Identity
            [Required(ErrorMessage = "وارد کردن ایمیل الزامی است.")]
            [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست.")]
            [Display(Name = "ایمیل")]
            public string Email { get; set; }

            [Required(ErrorMessage = "وارد کردن کلمه عبور الزامی است.")]
            [StringLength(100, ErrorMessage = "کلمه عبور باید حداقل {2} و حداکثر {1} کاراکتر باشد.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "کلمه عبور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تکرار کلمه عبور")]
            [Compare("Password", ErrorMessage = "کلمه عبور و تکرار آن یکسان نیستند.")]
            public string ConfirmPassword { get; set; }
        }

        /// <summary>
        /// این متد زمانی اجرا می‌شود که صفحه برای اولین بار با متد GET درخواست می‌شود (یعنی وقتی کاربر وارد صفحه ثبت‌نام می‌شود).
        /// وظیفه آن آماده‌سازی صفحه است.
        /// </summary>
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        /// <summary>
        /// این متد زمانی اجرا می‌شود که کاربر فرم ثبت‌نام را پر کرده و دکمه "ثبت نام" را کلیک می‌کند (درخواست POST).
        /// این متد قلب منطق ثبت‌نام است.
        /// </summary>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // ModelState.IsValid بررسی می‌کند که آیا اطلاعات وارد شده توسط کاربر،
            // با قوانین اعتبارسنجی که در InputModel تعریف کردیم (مثل Required, EmailAddress) مطابقت دارد یا خیر.
            if (ModelState.IsValid)
            {
                // یک نمونه جدید از کاربر سفارشی خودمان می‌سازیم
                var user = CreateUser();

                // اطلاعات وارد شده توسط کاربر را به پراپرتی‌های آبجکت user نسبت می‌دهیم
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.PhoneNumber;

                // نام کاربری و ایمیل را تنظیم می‌کنیم
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // با استفاده از UserManager، تلاش می‌کنیم کاربر جدید را با کلمه عبور داده شده در دیتابیس بسازیم
                var result = await _userManager.CreateAsync(user, Input.Password);

                // اگر ساخت کاربر با موفقیت انجام شد
                if (result.Succeeded)
                {
                    _logger.LogInformation("کاربر جدید با کلمه عبور ایجاد شد.");

                    // اگر نیاز به تایید ایمیل باشد، کد تایید را ساخته و ایمیل را ارسال می‌کنیم
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "حساب کاربری خود را تایید کنید",
                        $"لطفا با کلیک روی این لینک حساب خود را تایید کنید: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک کنید</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // اگر نیازی به تایید ایمیل نبود، کاربر را مستقیماً وارد سیستم می‌کنیم
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                // اگر ساخت کاربر با خطا مواجه شد (مثلا ایمیل تکراری بود)
                foreach (var error in result.Errors)
                {
                    // خطاها را به ModelState اضافه می‌کنیم تا در صفحه به کاربر نمایش داده شود
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // اگر ModelState معتبر نبود، یعنی کاربر فرم را اشتباه پر کرده است.
            // صفحه را دوباره به همراه پیام‌های خطا به او نمایش می‌دهیم.
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                // یک نمونه جدید از کلاس کاربر سفارشی ما (ApplicationUser) می‌سازد
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"امکان ساخت نمونه‌ای از '{nameof(ApplicationUser)}' وجود ندارد. " +
                    $"مطمئن شوید که این کلاس abstract نیست و یک سازنده بدون پارامتر دارد.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("رابط کاربری پیش‌فرض به UserStore با پشتیبانی از ایمیل نیاز دارد.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}

