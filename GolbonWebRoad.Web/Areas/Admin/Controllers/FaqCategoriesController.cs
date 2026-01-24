using AutoMapper;
using GolbonWebRoad.Application.Dtos.Faqs;
using GolbonWebRoad.Application.Features.Faqs.Commands;
using GolbonWebRoad.Application.Features.Faqs.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.Faqs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FaqCategoriesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public FaqCategoriesController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

        public async Task<IActionResult> Index()
        {
            var list = await _mediator.Send(new GetFaqCategoriesQuery { OnlyActive = false });
            var vm = list.Select(c => new FaqCategoryViewModel { Id = c.Id, Name = c.Name, Slog = c.Slog, SortOrder = c.SortOrder, IsActive = c.IsActive }).ToList();
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View(new FaqCategoryViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] FaqCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _mediator.Send(new CreateFaqCategoryCommand { Name = model.Name, Slog = model.Slog, SortOrder = model.SortOrder, IsActive = model.IsActive });
            TempData["SuccessMessage"] = "دسته ایجاد شد";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = (await _mediator.Send(new GetFaqCategoriesQuery { OnlyActive = false })).FirstOrDefault(x => x.Id == id);
            if (dto == null) return NotFound();
            return View(new FaqCategoryViewModel { Id = dto.Id, Name = dto.Name, Slog = dto.Slog, SortOrder = dto.SortOrder, IsActive = dto.IsActive });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] FaqCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _mediator.Send(new UpdateFaqCategoryCommand { Id = model.Id, Name = model.Name, Slog = model.Slog, SortOrder = model.SortOrder, IsActive = model.IsActive });
            TempData["SuccessMessage"] = "ویرایش شد";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = (await _mediator.Send(new GetFaqCategoriesQuery { OnlyActive = false })).FirstOrDefault(x => x.Id == id);
            if (dto == null) return NotFound();
            return View(new FaqCategoryViewModel { Id = dto.Id, Name = dto.Name });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeleteFaqCategoryCommand { Id = id });
            TempData["SuccessMessage"] = "حذف شد";
            return RedirectToAction(nameof(Index));
        }
    }
}
