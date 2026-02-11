using AutoMapper;
using GolbonWebRoad.Application.Features.BlogCategories.Queries;
using GolbonWebRoad.Web.Models.BlogCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Controllers
{
    public class BlogCategoriesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BlogCategoriesController(IMediator mediator, IMapper mapper)
        {
            _mapper=mapper;
            _mediator=mediator;
        }
        public async Task<IActionResult> Index()
        {
            var blogsEntity = await _mediator.Send(new GetAllBlogCategoryQuery());
            var blogCategoryViewModel = _mapper.Map<BlogCategorySummaryViewModel>(blogsEntity);
            return View();
        }
        [HttpGet("blogcategory")]
        public async Task<IActionResult> Detail(int id)
        {
            var blogCategoryEntity = await _mediator.Send(new GetByIdBlogCategoryQuery { Id=id });
            var blogCategoryViewModel = _mapper.Map<BlogCategoryViewModel>(blogCategoryEntity);
            return View(blogCategoryViewModel);

        }
    }
}
