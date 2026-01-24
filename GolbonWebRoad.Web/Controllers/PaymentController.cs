using GolbonWebRoad.Web.Models.Payments.Payping;
using GolbonWebRoad.Web.Services.Payments;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaypingSandboxGateway _payment;
        private readonly IConfiguration _config;

        public PaymentController(IConfiguration config)
        {
            var http = new HttpClient();
            var option = new PaypingOptions();
            _payment=new PaypingSandboxGateway(http, option);
            _config = config;
        }



        [HttpPost]


        public async Task<IActionResult> Start([FromForm] CreatePaymentRequestModel model, CancellationToken ct)
        {

            CreatePaymentResultModel viewModel = await _payment.StartAsync(model, ct);
            return View(viewModel);

        }








        //ساخت دی تی او برای دریافت اطلاعات موقع ریدایرکت شدن کاربر به این یو ار ال

        //[HttpPost]
        //[AllowAnonymous]
        //[IgnoreAntiforgeryToken]

        //public async Task<IActionResult> Callback([FromForm] CreatePaymentResultModel model)
        //{
        //    Console.Write("PaymentCode:");
        //    Console.WriteLine(model.PaymentCode);
        //    Console.Write("PaymentUrl:");
        //    Console.WriteLine(model.PaymentUrl);
        //    Console.Write("Amount:");
        //    Console.WriteLine(model.Amount);
        //    Console.Write("BusinessWage:");
        //    Console.WriteLine(model.BusinessWage);
        //    Console.Write("PayerWage:");
        //    Console.WriteLine(model.PayerWage);
        //    return View(model);


        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public async Task<IActionResult> Start(string orderId, long amount, string provider = "Aqayepardakht")
        //{
        //    // این متد بدون تغییر باقی می‌ماند
        //    var callback = Url.Action("Callback", "Payment", null, Request.Scheme);
        //    //var callback = Url.Action("InspectRequest", "Payment", null, Request.Scheme);

        //    var gateway = _resolver.Get(provider);
        //    var res = await gateway.StartAsync(new PaymentStartRequest
        //    {
        //        OrderId = orderId,
        //        Amount = amount,
        //        CallbackUrl = callback,
        //        Description = $"Order {orderId}",
        //        CustomerMobile = User?.Identity?.Name
        //    });

        //    if (!res.IsSuccessful)
        //    {
        //        TempData["PaymentError"] = res.Message;
        //        return RedirectToAction("Success", "Cart");
        //    }
        //    return Redirect(res.GatewayRedirectUrl);
    }


    //[HttpPost] // فقط درخواست‌های POST را می‌پذیریم
    //[AllowAnonymous]
    //[IgnoreAntiforgeryToken]
    //public async Task<IActionResult> Callback([FromForm] AghayePardakhtCallbackModel model)
    //{
    //    // بررسی می‌کنیم که آیا پارامترهای اصلی ارسال شده‌اند یا خیر
    //    if (model == null || string.IsNullOrEmpty(model.Status) || string.IsNullOrEmpty(model.TransactionId))
    //    {
    //        TempData["PaymentError"] = "اطلاعات بازگشتی از درگاه پرداخت ناقص است.";
    //        return RedirectToAction("Index", "Checkout");
    //    }

    //    // طبق مستندات آقای پرداخت، status برابر با '1' به معنی موفقیت است
    //    if (model.Status != "1")
    //    {
    //        return RedirectToAction("Success", "Checkout", new CallbackMessageViewModel { PaymentStatus=false, MessageTransAction="تراکنش ناموفق", MessageCreateOrder="سفارش ثبت نشد", TransactionId=model.TransactionId, OrderId=model.OrderId });
    //    }
    //    TempData["PaymentSuccess"] = true;
    //    TempData["TransactionId"] = model.TransactionId;
    //    // پس از پرداخت موفق، سفارش را ثبت کن و سبد را خالی کن

    //    return RedirectToAction("PlaceOrder", "Checkout", new CallbackMessageViewModel { PaymentStatus=true, MessageTransAction="تراکنش با موفقیت انجام شد", MessageCreateOrder="", TransactionId=model.TransactionId, OrderId=model.OrderId });
    //}




    //[HttpPost, HttpGet] // هم POST و هم GET را قبول می‌کند تا چیزی را از دست ندهیم
    //[AllowAnonymous]
    //[IgnoreAntiforgeryToken]
    //[Route("Payment/InspectRequest")] // یک آدرس جدید برای این اکشن
    //    public async Task<IActionResult> InspectRequest()
    //    {
    //        // از سرویس لاگر برای نمایش خروجی در کنسول ویژوال استودیو استفاده می‌کنیم
    //        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<PaymentController>>();

    //        logger.LogInformation("--- شروع بازرسی درخواست خام ورودی ---");
    //        logger.LogInformation("متد درخواست: {Method}", Request.Method);
    //        logger.LogInformation("Content-Type: {ContentType}", Request.ContentType);

    //        // اگر درخواست از نوع POST و حاوی Form Data باشد
    //        if (Request.HasFormContentType)
    //        {
    //            // فرم را می‌خوانیم
    //            var formCollection = await Request.ReadFormAsync();

    //            logger.LogInformation("داده‌های فرم دریافت شده:");
    //            foreach (var key in formCollection.Keys)
    //            {
    //                // کلید و مقدار هر پارامتر را چاپ می‌کنیم
    //                logger.LogInformation("-> Key: \"{Key}\", Value: \"{Value}\"", key, formCollection[key]);
    //            }
    //        }
    //        else
    //        {
    //            logger.LogWarning("درخواست ورودی حاوی Form Data نبود.");
    //        }

    //        logger.LogInformation("--- پایان بازرسی ---");

    //        // یک پاسخ ساده برمی‌گردانیم
    //        return Ok("Request was inspected. Check logs.");
    //    }
    //}

}