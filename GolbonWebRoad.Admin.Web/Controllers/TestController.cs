using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Admin.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public TestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory=httpClientFactory;
        }

    }
}
