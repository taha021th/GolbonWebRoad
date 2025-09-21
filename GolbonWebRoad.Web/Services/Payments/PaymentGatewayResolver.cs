namespace GolbonWebRoad.Web.Services.Payments
{
    public interface IPaymentGatewayResolver
    {
        IPaymentGateway Get(string name);
        IPaymentGateway Default();
    }

    public class PaymentGatewayResolver : IPaymentGatewayResolver
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;
        public PaymentGatewayResolver(IEnumerable<IPaymentGateway> gateways)
        {
            _gateways = gateways;
        }
        public IPaymentGateway Get(string name) => _gateways.FirstOrDefault(g => string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase)) ?? _gateways.First();
        public IPaymentGateway Default() => _gateways.First();
    }
}


