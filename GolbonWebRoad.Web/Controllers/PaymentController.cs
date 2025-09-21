using GolbonWebRoad.Web.Models.Transactions;
using GolbonWebRoad.Web.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentGatewayResolver _resolver;
        private readonly IConfiguration _config;

        public PaymentController(IPaymentGatewayResolver resolver, IConfiguration config)
        {
            _resolver = resolver;
            _config = config;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Start(string orderId, long amount, string provider = "Aqayepardakht")
        {
            // این متد بدون تغییر باقی می‌ماند
            var callback = Url.Action("Callback", "Payment", null, Request.Scheme);
            //var callback = Url.Action("InspectRequest", "Payment", null, Request.Scheme);

            var gateway = _resolver.Get(provider);
            var res = await gateway.StartAsync(new PaymentStartRequest
            {
                OrderId = orderId,
                Amount = amount,
                CallbackUrl = callback,
                Description = $"Order {orderId}",
                CustomerMobile = User?.Identity?.Name
            });

            if (!res.IsSuccessful)
            {
                TempData["PaymentError"] = res.Message;
                return RedirectToAction("Success", "Cart");
            }
            return Redirect(res.GatewayRedirectUrl);
        }

        // مدل برای دریافت پارامترهای بازگشتی از آقای پرداخت
        public class AghayePardakhtCallbackModel
        {
            [FromForm(Name = "transid")]
            public string TransactionId { get; set; }

            // این خط اصلاح شد
            [FromForm(Name = "invoice_id")] // قبلاً "order_id" بود
            public string OrderId { get; set; }

            [FromForm(Name = "amount")]
            public string Amount { get; set; }

            [FromForm(Name = "status")]
            public string Status { get; set; }

            [FromForm(Name = "card_number")]
            public string CardNumber { get; set; }
        }


        [HttpPost] // فقط درخواست‌های POST را می‌پذیریم
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Callback([FromForm] AghayePardakhtCallbackModel model)
        {
            // بررسی می‌کنیم که آیا پارامترهای اصلی ارسال شده‌اند یا خیر
            if (model == null || string.IsNullOrEmpty(model.Status) || string.IsNullOrEmpty(model.TransactionId))
            {
                TempData["PaymentError"] = "اطلاعات بازگشتی از درگاه پرداخت ناقص است.";
                return RedirectToAction("Index", "Checkout");
            }

            // طبق مستندات آقای پرداخت، status برابر با '1' به معنی موفقیت است
            if (model.Status != "1")
            {
                return RedirectToAction("Success", "Checkout", new CallbackMessageViewModel { PaymentStatus=false, MessageTransAction="تراکنش ناموفق", MessageCreateOrder="سفارش ثبت نشد", TransactionId=model.TransactionId, OrderId=model.OrderId });
            }
            TempData["PaymentSuccess"] = true;
            TempData["TransactionId"] = model.TransactionId;
            // پس از پرداخت موفق، سفارش را ثبت کن و سبد را خالی کن

            return RedirectToAction("PlaceOrder", "Checkout", new CallbackMessageViewModel { PaymentStatus=true, MessageTransAction="تراکنش با موفقیت انجام شد", MessageCreateOrder="", TransactionId=model.TransactionId, OrderId=model.OrderId });
        }




        [HttpPost, HttpGet] // هم POST و هم GET را قبول می‌کند تا چیزی را از دست ندهیم
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [Route("Payment/InspectRequest")] // یک آدرس جدید برای این اکشن
        public async Task<IActionResult> InspectRequest()
        {
            // از سرویس لاگر برای نمایش خروجی در کنسول ویژوال استودیو استفاده می‌کنیم
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<PaymentController>>();

            logger.LogInformation("--- شروع بازرسی درخواست خام ورودی ---");
            logger.LogInformation("متد درخواست: {Method}", Request.Method);
            logger.LogInformation("Content-Type: {ContentType}", Request.ContentType);

            // اگر درخواست از نوع POST و حاوی Form Data باشد
            if (Request.HasFormContentType)
            {
                // فرم را می‌خوانیم
                var formCollection = await Request.ReadFormAsync();

                logger.LogInformation("داده‌های فرم دریافت شده:");
                foreach (var key in formCollection.Keys)
                {
                    // کلید و مقدار هر پارامتر را چاپ می‌کنیم
                    logger.LogInformation("-> Key: \"{Key}\", Value: \"{Value}\"", key, formCollection[key]);
                }
            }
            else
            {
                logger.LogWarning("درخواست ورودی حاوی Form Data نبود.");
            }

            logger.LogInformation("--- پایان بازرسی ---");

            // یک پاسخ ساده برمی‌گردانیم
            return Ok("Request was inspected. Check logs.");
        }
    }

}