using GolbonWebRoad.Application.Dtos.Users;
using GolbonWebRoad.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GolbonWebRoad.Application.Features.Users.Queries
{
    public class GetUserRolesQuery : IRequest<ManageUserRolesDto>
    {
        public string UserId { get; set; }
    }
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, ManageUserRolesDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public GetUserRolesQueryHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }
        public async Task<ManageUserRolesDto> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return null;
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync(cancellationToken);
            var rolesDto = allRoles.Select(role
                => new RoleDto
                {
                    RoleName = role.Name,
                    IsSelected = userRoles.Contains(role.Name)
                }).ToList();
            return new ManageUserRolesDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = rolesDto
            };


        }
    }
}
