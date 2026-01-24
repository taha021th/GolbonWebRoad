using AutoMapper;
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
    public class FaqsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public FaqsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator; _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await _mediator.Send(new GetFaqsQuery { OnlyActive = false });
            var vm = list.Select(_mapper.Map<FaqViewModel>).ToList();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateFaqViewModel();
            await PopulateCategories(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateFaqViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategories(model);
                return View(model);
            }
            var cmd = _mapper.Map<CreateFaqCommand>(model);
            cmd.FaqCategoryId = model.CategoryId;
            await _mediator.Send(cmd);
            TempData["SuccessMessage"] = "سوال با موفقیت ایجاد شد";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _mediator.Send(new GetFaqByIdQuery { Id = id });
            var vm = new EditFaqViewModel
            {
                Id = dto.Id,
                Slog = dto.Slog,
                Question = dto.Question,
                Answer = dto.Answer,
                CategoryId = dto.FaqCategoryId,
                Tags = dto.Tags,
                SortOrder = dto.SortOrder,
                IsActive = dto.IsActive
            };
            await PopulateCategories(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] EditFaqViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategories(model);
                return View(model);
            }
            var cmd = _mapper.Map<UpdateFaqCommand>(model);
            cmd.FaqCategoryId = model.CategoryId;
            await _mediator.Send(cmd);
            TempData["SuccessMessage"] = "سوال با موفقیت ویرایش شد";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _mediator.Send(new GetFaqByIdQuery { Id = id });
            var vm = _mapper.Map<FaqViewModel>(dto);
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeleteFaqCommand { Id = id });
            TempData["SuccessMessage"] = "سوال حذف شد";
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateCategories(CreateFaqViewModel vm)
        {
            var cats = await _mediator.Send(new GetFaqCategoriesQuery { OnlyActive = false });
            vm.CategoryOptions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(cats, "Id", "Name", vm.CategoryId);
        }
        private async Task PopulateCategories(EditFaqViewModel vm)
        {
            var cats = await _mediator.Send(new GetFaqCategoriesQuery { OnlyActive = false });
            vm.CategoryOptions = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(cats, "Id", "Name", vm.CategoryId);
        }
    }
}
