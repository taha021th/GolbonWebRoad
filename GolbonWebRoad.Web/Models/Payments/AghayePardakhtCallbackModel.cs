using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Models.Payments
{
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
}
