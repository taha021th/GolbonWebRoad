namespace GolbonWebRoad.Application.Dtos.Users
{
    public class ManageUserRolesDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<RoleDto> Roles { get; set; }
    }
}
