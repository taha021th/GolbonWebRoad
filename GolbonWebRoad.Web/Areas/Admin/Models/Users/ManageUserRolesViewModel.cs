using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolbonWebRoad.Web.Areas.Admin.Models.Users
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public IList<string> UserRoles { get; set; }
    }
}
