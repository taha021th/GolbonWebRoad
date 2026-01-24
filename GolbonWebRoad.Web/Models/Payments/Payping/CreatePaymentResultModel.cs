using System.Text.Json.Serialization;

namespace GolbonWebRoad.Web.Models.Payments.Payping
{
    public class CreatePaymentResultModel
    {
        [JsonPropertyName("paymentCode")]
        public string PaymentCode { get; set; }

        [JsonPropertyName("url")]
        public string PaymentUrl { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; } // int بر اساس مستندات

        [JsonPropertyName("payerWage")]
        public int PayerWage { get; set; } // int بر اساس مستندات

        [JsonPropertyName("businessWage")]
        public int BusinessWage { get; set; } // int بر اساس مستندات

        [JsonPropertyName("gatewayAmount")]
        public int GatewayAmount { get; set; } // int بر اساس مستندات
    }

}
