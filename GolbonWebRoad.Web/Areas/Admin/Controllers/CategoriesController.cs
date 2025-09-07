using AutoMapper;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public CategoriesController(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _mediator.Send(new GetCategoriesQuery());
            return View(categories);
        }
        public IActionResult Create()
        {
            var viewModel = new CreateCategoryViewModel();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var command = _mapper.Map<CreateCategoryCommand>(model);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var categoryDto = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
            if (categoryDto == null) return NotFound();
            var viewModel = _mapper.Map<EditCategoryViewModel>(categoryDto);
            return View(viewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var command = _mapper.Map<UpdateCategoryCommand>(model);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int id)
        {
            var categoryDto = await _mediator.Send(new GetCategoryByIdQuery { Id=id });
            if (categoryDto==null) return NotFound();

            return View(categoryDto);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeleteCategoryCommand { Id=id });
            return RedirectToAction(nameof(Index));
        }
    }
}
