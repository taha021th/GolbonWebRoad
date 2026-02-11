using AutoMapper;
using GolbonWebRoad.Application.Features.BlogReviews.Commands;
using GolbonWebRoad.Application.Features.Blogs.Queries;
using GolbonWebRoad.Web.Models.BlogReviews;
using GolbonWebRoad.Web.Models.Blogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GolbonWebRoad.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BlogsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper=mapper;
        }
        [Route("blogs")]
        public async Task<IActionResult> Index()
        {
            var blogsEntity = await _mediator.Send(new GetAllBlogQuery());
            var viewModel = _mapper.Map<List<BlogSummaryViewModel>>(blogsEntity);
            return View(viewModel);
        }
        [Route("blog/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {

            var blogEntity = await _mediator.Send(new GetByIdBlogQuery { Id=id });
            var viewModel = _mapper.Map<BlogViewModel>(blogEntity);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(BlogReviewViewModel viewModel)
        {

            //if (!ModelState.IsValid)
            //{
            //    TempData["Error"]="لطفا تمام فیلد هارا به درستی پر کنید.";
            //    return RedirectToAction("Detail", new { id = viewModel.BlogId });
            //}

            var command = _mapper.Map<CreateBlogReviewCommand>(viewModel);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            command.UserId=userId;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "لطفا ابتدا وارد حساب کاربری خود شوید";
                return RedirectToAction("Detail", new { id = viewModel.BlogId });
            }

            await _mediator.Send(command);
            return RedirectToAction("Detail", new { Id = viewModel.BlogId });
        }

    }
}
