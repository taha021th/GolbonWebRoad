using AutoMapper;
using GolbonWebRoad.Application.Features.BlogCategories.Queries;
using GolbonWebRoad.Application.Features.Blogs.Commands;
using GolbonWebRoad.Application.Features.Blogs.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.BlogCategories;
using GolbonWebRoad.Web.Areas.Admin.Models.Blogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BlogsController(IMediator mediator, IMapper mapper)
        {
            _mediator=mediator;
            _mapper=mapper;
        }
        public async Task<IActionResult> Index()
        {
            var blogEntity = await _mediator.Send(new GetAllBlogQuery());
            var viewModel = _mapper.Map<List<BlogViewModel>>(blogEntity);
            return View(viewModel);
        }
        public async Task<IActionResult> Create()
        {
            var blogCategoriesEntity = await _mediator.Send(new GetAllBlogCategoryQuery());
            var viewModel = new CreateBlogViewModel();
            viewModel.BlogCategoryListItemViewModels= _mapper.Map<List<BlogCategoryListItemViewModel>>(blogCategoriesEntity);
            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBlogViewModel viewModel)
        {
            var blogCommand = _mapper.Map<CreateBlogCommand>(viewModel);
            await _mediator.Send(blogCommand);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var blogCategoriesEntity = await _mediator.Send(new GetAllBlogCategoryQuery());

            var entity = await _mediator.Send(new GetByIdBlogQuery { Id=id });
            var viewModel = _mapper.Map<EditBlogViewModel>(entity);
            viewModel.BlogCategoryListItemViewModels= _mapper.Map<List<BlogCategoryListItemViewModel>>(blogCategoriesEntity);
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Update(EditBlogViewModel viewModel)
        {
            var blogCommand = _mapper.Map<UpdateBlogCommand>(viewModel);
            await _mediator.Send(blogCommand);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {

            await _mediator.Send(new DeleteBlogCommand { Id=id });
            return RedirectToAction("Index");
        }

    }
}
