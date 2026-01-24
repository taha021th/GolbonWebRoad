namespace GolbonWebRoad.Web.Models.Payments.Payping
{
    public class CreatePaymentRequestModel
    {
        // PayPing این را int می‌خواهد
        public int Amount { get; set; }

        public string ReturnUrl { get; set; }

        // فیلدهای اختیاری باید Nullable باشند
        public string? PayerIdentity { get; set; }

        public string? PayerName { get; set; }

        public string? Description { get; set; }

        public string? ClientRefId { get; set; }

        public string? NationalCode { get; set; }

        // PayPing این را bool می‌خواهد
        public bool IsReversible { get; set; }
    }
}
