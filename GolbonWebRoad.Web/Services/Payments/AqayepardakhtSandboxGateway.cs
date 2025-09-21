namespace GolbonWebRoad.Web.Services.Payments
{
    public class AqayepardakhtOptions
    {
        public string MerchantId { get; set; }
        public string Pin { get; set; } = "sandbox"; // sandbox pin per provider docs
        public string BaseUrl { get; set; } = "https://panel.aqayepardakht.ir";
        public string StartPath { get; set; } = "/api/v2/create"; // create on panel
        public string VerifyPath { get; set; } = "/api/v2/verify"; // verify on panel
        public string RedirectTemplate { get; set; } = "/startpay/sandbox/{transid}";
    }

    public class AqayepardakhtSandboxGateway : IPaymentGateway
    {
        private readonly HttpClient _http;
        private readonly AqayepardakhtOptions _options;

        public AqayepardakhtSandboxGateway(HttpClient http, AqayepardakhtOptions options)
        {
            _http = http;
            _options = options;

            if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                _http.BaseAddress = new Uri(_options.BaseUrl);
            }
        }

        public string Name => "Aqayepardakht";

        public async Task<PaymentStartResult> StartAsync(PaymentStartRequest request, CancellationToken ct = default)
        {
            var payload = new
            {
                pin = _options.Pin,
                amount = request.Amount,
                callback = request.CallbackUrl,
                invoice_id = request.OrderId,
                description = request.Description,
                mobile = request.CustomerMobile
            };

            var resp = await _http.PostAsJsonAsync(_options.StartPath, payload, ct);
            if (!resp.IsSuccessStatusCode)
            {
                return new PaymentStartResult { IsSuccessful = false, Message = $"HTTP {(int)resp.StatusCode}" };
            }

            var data = await resp.Content.ReadFromJsonAsync<AqStartResponse>(cancellationToken: ct);
            if (IsSuccess(data?.status))
            {
                var redirect = $"{_options.BaseUrl}{_options.RedirectTemplate.Replace("{transid}", data.transid ?? string.Empty)}";

                return new PaymentStartResult
                {
                    IsSuccessful = true,
                    Authority = data.transid,
                    GatewayRedirectUrl = redirect
                };
            }

            return new PaymentStartResult { IsSuccessful = false, Message = data?.message ?? "Unknown error" };
        }

        public async Task<PaymentVerifyResult> VerifyAsync(PaymentVerifyRequest request, CancellationToken ct = default)
        {
            var payload = new
            {
                pin = _options.Pin,
                amount = request.Amount,
                transid = request.Authority,
                order_id = request.OrderId
            };

            var resp = await _http.PostAsJsonAsync(_options.VerifyPath, payload, ct);
            if (!resp.IsSuccessStatusCode)
            {
                return new PaymentVerifyResult { IsSuccessful = false, Message = $"HTTP {(int)resp.StatusCode}" };
            }

            var data = await resp.Content.ReadFromJsonAsync<AqVerifyResponse>(cancellationToken: ct);
            if (IsSuccess(data?.status))
            {
                return new PaymentVerifyResult
                {
                    IsSuccessful = true,
                    RefId = data.tracking_number,
                    Message = "تراکنش با موفقیت تایید شد"
                };
            }

            return new PaymentVerifyResult { IsSuccessful = false, Message = data?.message ?? "Verify failed" };
        }

        private static bool IsSuccess(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;
            if (int.TryParse(status, out var s)) return s == 1;

            return string.Equals(status, "success", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase);
        }

        private sealed class AqStartResponse
        {
            public string status { get; set; }
            public string transid { get; set; }
            public string message { get; set; }
        }

        private sealed class AqVerifyResponse
        {
            public string status { get; set; }
            public string tracking_number { get; set; } // تغییر داده شد
            public string message { get; set; }
        }
    }
}
