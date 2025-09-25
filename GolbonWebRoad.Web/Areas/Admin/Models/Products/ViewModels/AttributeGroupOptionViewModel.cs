using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels
{
    public class AttributeGroupOptionViewModel
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public List<SelectListItem> Values { get; set; } = new List<SelectListItem>();
    }
}


