namespace GolbonWebRoad.Web.Areas.Admin.Models.Users
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<RoleViewModel> Roles { get; set; } // <<-- این خط تغییر کرد
        public IList<string> UserRoles { get; set; }
    }
}
