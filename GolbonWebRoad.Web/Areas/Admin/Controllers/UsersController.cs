using AutoMapper;
using GolbonWebRoad.Application.Features.Users.Commands;
using GolbonWebRoad.Application.Features.Users.Queries;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Web.Areas.Admin.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UsersController(IUserService userService, IMediator mediator, IMapper mapper)
        {
            _userService=userService;
            _mediator=mediator;
            _mapper=mapper;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }
        public async Task<IActionResult> ManageRoles(string id)
        {

            var userRolesDto = await _mediator.Send(new GetUserRolesQuery { UserId=id });
            if (userRolesDto==null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<ManageUserRolesViewModel>(userRolesDto);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
        {

            var command = _mapper.Map<UpdateUserRoleCommand>(model);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
    }

}







