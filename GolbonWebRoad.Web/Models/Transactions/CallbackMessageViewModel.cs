namespace GolbonWebRoad.Web.Models.Transactions
{
    public class CallbackMessageViewModel
    {
        public bool PaymentStatus { get; set; }
        public string MessageTransAction { get; set; }
        public string MessageCreateOrder { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
    }
}
