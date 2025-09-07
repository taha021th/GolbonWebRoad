using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolbonWebRoad.Admin.Web.ViewModels
{
    public class UpdateOrderStatusViewModel
    {
        public int OrderId { get; set; }
        public string CurrentStatus { get; set; }
        public string NewStatus { get; set; }

        public List<SelectListItem> AvailableStatuses { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "در حال پردازش", Text = "در حال پردازش" },
            new SelectListItem { Value = "ارسال شده", Text = "ارسال شده" },
            new SelectListItem { Value = "تحویل داده شده", Text = "تحویل داده شده" },
            new SelectListItem { Value = "لغو شده", Text = "لغو شده" }
        };
    }
}
