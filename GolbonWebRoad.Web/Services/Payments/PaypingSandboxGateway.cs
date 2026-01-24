
using GolbonWebRoad.Web.Models.Payments.Payping;
using System.Net.Http.Headers;

namespace GolbonWebRoad.Web.Services.Payments
{

    public class PaypingOptions
    {

        public string BaseUrl { get; set; } = "https://8jg0srqj-7162.euw.devtunnels.ms/";
        public string CreatePaymentPath { get; set; } = "https://api.payping.ir/v3/pay";

    }

    public class PaypingSandboxGateway
    {
        private readonly HttpClient _http;
        private readonly PaypingOptions _options;
        public PaypingSandboxGateway(HttpClient http, PaypingOptions options)
        {
            _http=http;
            _options=options;
        }
        public string Name => throw new NotImplementedException();

        public async Task<CreatePaymentResultModel> StartAsync(CreatePaymentRequestModel request, CancellationToken ct = default)
        {



            _http.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", "DFA580A6581920A39AE63D9D08870A94D025A818241B1290BCE11FA9935AF7F5-1");
            var response = await _http.PostAsJsonAsync(_options.CreatePaymentPath, request, ct);

            if (!response.IsSuccessStatusCode)
            {
                // در صورت عدم موفقیت (مثلاً 404 یا 500)، محتوای خطا را نمایش می‌دهد
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Request Failed with Status {response.StatusCode}. Content: {errorContent}");

                // در اینجا باید مدیریت خطا را انجام دهید، مثلاً throw کردن Exception
                throw new HttpRequestException($"Payping API request failed. Status: {response.StatusCode}");
            }
            var data = await response.Content.ReadFromJsonAsync<CreatePaymentResultModel>(ct);
            Console.WriteLine(data.PaymentCode);
            Console.WriteLine(data.PaymentUrl);
            Console.WriteLine(data.GatewayAmount);
            Console.WriteLine(data.Amount);
            Console.WriteLine(data.BusinessWage);
            Console.WriteLine(data.PayerWage);


            return data;
        }

        //public Task<PaymentVerifyResult> VerifyAsync(PaymentVerifyRequest request, CancellationToken ct = default)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
