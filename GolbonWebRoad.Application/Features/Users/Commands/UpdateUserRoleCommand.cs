using GolbonWebRoad.Application.Dtos.Users;
using GolbonWebRoad.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GolbonWebRoad.Application.Features.Users.Commands
{
    public class UpdateUserRoleCommand : IRequest
    {
        public string UserId { get; set; }
        public List<RoleDto> Roles { get; set; }

    }
    public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRoleCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UpdateUserRolesCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager=userManager;
        }
        public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user==null) return;
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            var selectedRoles = request.Roles.Where(r => r.IsSelected).Select(r => r.RoleName);
            await _userManager.AddToRolesAsync(user, selectedRoles);
        }
    }
}
