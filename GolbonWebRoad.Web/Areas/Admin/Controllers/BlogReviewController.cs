using AutoMapper;
using GolbonWebRoad.Application.Features.BlogReviews.Commands;
using GolbonWebRoad.Application.Features.BlogReviews.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.BlogReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogReviewController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BlogReviewController(IMediator mediator, IMapper mapper)
        {
            _mediator= mediator;
            _mapper=mapper;
        }
        public async Task<IActionResult> Index(string status = "all")
        {
            bool? statusFilter = status switch
            {
                "pending" => false,
                "approved" => true,
                _ => null
            };

            var blogReviews = await _mediator.Send(new GetAllBlogReviewsForAdminQuery
            {
                Status=statusFilter
            });
            var allReviews = await _mediator.Send(new GetAllBlogReviewsForAdminQuery
            {
                Status=null
            });
            var viewModel = new BlogReviewIndexViewModel
            {
                Reviews = _mapper.Map<List<BlogReviewViewModel>>(blogReviews),
                TotalReviews = allReviews.Count(),
                PendingReviews=allReviews.Count(r => !r.Status),
                ApprovedReviews=allReviews.Count(r => r.Status),
                FilterStatus=status
            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int reviewId, bool status)
        {
            try
            {
                await _mediator.Send(new UpdateStatusBlogReviewCommand
                {
                    Id=reviewId,
                    Status=status
                });
                var statusText = status ? "تایید" : "رد";
                TempData["Success"]=$"نظر با موفقیت {statusText} شد.";
            }
            catch (Exception ex)
            {
                TempData["Error"]="خطایی در بروزرسانی وضغیت نظر رخ داد.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpdateStatus(int[] reviewIds, bool status)
        {

            try
            {
                foreach (var reviewId in reviewIds)
                {
                    await _mediator.Send(new UpdateStatusBlogReviewCommand
                    {
                        Id=reviewId,
                        Status=status
                    });
                }
                var statusText = status ? "تایید" : "رد";
                TempData["Success"]=$"{reviewIds.Length} نظر با موفقیت {statusText} شد.";
            }
            catch (Exception ex)
            {
                TempData["Error"]="خطایی در بروزرسانی دسته ای نظرات رخ داد";
            }

            return RedirectToAction("Index");
        }
    }
}
