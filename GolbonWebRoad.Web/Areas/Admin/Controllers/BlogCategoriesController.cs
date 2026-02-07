using AutoMapper;
using GolbonWebRoad.Application.Features.BlogCategories.Commands;
using GolbonWebRoad.Application.Features.BlogCategories.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.BlogCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogCategoriesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BlogCategoriesController(IMediator mediator, IMapper mapper)
        {
            _mediator=mediator;
            _mapper=mapper;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _mediator.Send(new GetAllBlogCategoryQuery());
            var viewModel = _mapper.Map<List<BlogCategoryListItemViewModel>>(model);
            return View(viewModel);
        }
        public IActionResult Create()
        {

            return View(new BlogCategoryFromViewModel());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCategoryFromViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var command = _mapper.Map<CreateBlogCategoryCommand>(model);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Edit(int id)
        {
            var blogCategory = await _mediator.Send(new GetByIdBlogCategoryQuery { Id=id });
            var viewModel = _mapper.Map<BlogCategoryFromViewModel>(blogCategory);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BlogCategoryFromViewModel model)
        {
            var command = _mapper.Map<UpdateBlogCategoryCommand>(model);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteBlogCategoryCommand { Id=id });
            return RedirectToAction(nameof(Index));
        }
    }
}
