using AutoMapper;
using GolbonWebRoad.Application.Features.Blogs.Queries;
using GolbonWebRoad.Web.Models.Blogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BlogController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper=mapper;
        }
        public async Task<IActionResult> Index()
        {
            var blogEntity = await _mediator.Send(new GetAllBlogQuery());
            var viewModel = _mapper.Map<List<BlogSummaryViewModel>>(blogEntity);
            return View(viewModel);
        }
        public IActionResult Detail()
        {
            return View();
        }
        public IActionResult BlogCategories()
        {
            return View();
        }
    }
}
