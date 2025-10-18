using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Web.Models.Addresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Web.Controllers
{
    [Authorize]
    public class AddressesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddressesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _unitOfWork.UserAddressRepository.GetByUserIdAsync(userId);
            var vm = new AddressListViewModel
            {
                Items = list.Select(a => new AddressItemViewModel
                {
                    Id = a.Id,
                    UserFullName = $"{a.User.FirstName} {a.User.LastName}",
                    Phone = a.Phone,
                    AddressLine = a.AddressLine,
                    City = a.City,
                    Province = a.Province,
                    PostalCode = a.PostalCode,
                    IsDefault = a.IsDefault
                }).ToList()
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new AddressFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = new UserAddress
            {
                UserId = userId,
                Phone = model.Phone,
                AddressLine = model.AddressLine,
                City = model.City,
                Province = model.Province,
                PostalCode = model.PostalCode,
                IsDefault = model.IsDefault
            };
            await _unitOfWork.UserAddressRepository.AddAsync(entity);
            if (model.IsDefault)
            {
                var all = await _unitOfWork.UserAddressRepository.GetByUserIdAsync(userId);
                foreach (var a in all.Where(a => a.Id != entity.Id && a.IsDefault))
                {
                    a.IsDefault = false; _unitOfWork.UserAddressRepository.Update(a);
                }
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var addr = await _unitOfWork.UserAddressRepository.GetByIdAsync(id);
            if (addr == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (addr.UserId != userId) return Forbid();
            return View(new AddressFormViewModel
            {
                Id = addr.Id,
                Phone = addr.Phone,
                AddressLine = addr.AddressLine,
                City = addr.City,
                Province = addr.Province,
                PostalCode = addr.PostalCode,
                IsDefault = addr.IsDefault
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddressFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var addr = await _unitOfWork.UserAddressRepository.GetByIdAsync(model.Id!.Value);
            if (addr == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (addr.UserId != userId) return Forbid();

            addr.Phone = model.Phone;
            addr.AddressLine = model.AddressLine;
            addr.City = model.City;
            addr.Province = model.Province;
            addr.PostalCode = model.PostalCode;
            addr.IsDefault = model.IsDefault;
            _unitOfWork.UserAddressRepository.Update(addr);

            if (model.IsDefault)
            {
                var all = await _unitOfWork.UserAddressRepository.GetByUserIdAsync(userId);
                foreach (var a in all.Where(a => a.Id != addr.Id && a.IsDefault))
                {
                    a.IsDefault = false; _unitOfWork.UserAddressRepository.Update(a);
                }
            }

            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var addr = await _unitOfWork.UserAddressRepository.GetByIdAsync(id);
            if (addr == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (addr.UserId != userId) return Forbid();
            await _unitOfWork.UserAddressRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            var addr = await _unitOfWork.UserAddressRepository.GetByIdAsync(id);
            if (addr == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (addr.UserId != userId) return Forbid();
            addr.IsDefault = true;
            _unitOfWork.UserAddressRepository.Update(addr);
            var all = await _unitOfWork.UserAddressRepository.GetByUserIdAsync(userId);
            foreach (var a in all.Where(a => a.Id != id && a.IsDefault))
            {
                a.IsDefault = false; _unitOfWork.UserAddressRepository.Update(a);
            }
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}


