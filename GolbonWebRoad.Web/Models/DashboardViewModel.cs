using GolbonWebRoad.Application.Dtos.Orders;
using GolbonWebRoad.Web.Models.Addresses;

namespace GolbonWebRoad.Web.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<OrderDto>? Orders { get; set; }
        public IEnumerable<AddressItemViewModel>? Addresses { get; set; }

    }
}


