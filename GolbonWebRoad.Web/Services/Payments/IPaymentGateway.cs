namespace GolbonWebRoad.Web.Services.Payments
{
    public class PaymentStartRequest
    {
        public string OrderId { get; set; }
        public long Amount { get; set; }
        public string CallbackUrl { get; set; }
        public string Description { get; set; }
        public string CustomerMobile { get; set; }
    }

    public class PaymentStartResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string GatewayRedirectUrl { get; set; }
        public string Authority { get; set; }
    }

    public class PaymentVerifyRequest
    {
        public string Authority { get; set; }
        public long Amount { get; set; }
        public string OrderId { get; set; }
    }

    public class PaymentVerifyResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string RefId { get; set; }
    }

    public interface IPaymentGateway
    {
        string Name { get; }
        Task<PaymentStartResult> StartAsync(PaymentStartRequest request, CancellationToken ct = default);
        Task<PaymentVerifyResult> VerifyAsync(PaymentVerifyRequest request, CancellationToken ct = default);
    }
}


