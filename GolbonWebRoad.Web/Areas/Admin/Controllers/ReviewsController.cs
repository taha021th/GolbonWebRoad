using AutoMapper;
using GolbonWebRoad.Application.Features.Reviews.Queries;
using GolbonWebRoad.Application.Features.Reviews.Commands;
using GolbonWebRoad.Web.Areas.Admin.Models.Reviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ReviewsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string status = "all")
        {
            bool? statusFilter = status switch
            {
                "pending" => false,
                "approved" => true,
                _ => null
            };

            var reviews = await _mediator.Send(new GetAllReviewsWithDetailsQuery
            {
                Status = statusFilter
            });

            var allReviews = await _mediator.Send(new GetAllReviewsWithDetailsQuery
            {
                Status = null
            });

            var viewModel = new ReviewIndexViewModel
            {
                Reviews = _mapper.Map<List<ReviewViewModel>>(reviews),
                TotalReviews = allReviews.Count,
                PendingReviews = allReviews.Count(r => !r.Status),
                ApprovedReviews = allReviews.Count(r => r.Status),
                FilterStatus = status
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int reviewId, bool status)
        {
            try
            {
                await _mediator.Send(new UpdateReviewStatusCommand
                {
                    ReviewId = reviewId,
                    Status = status
                });

                var statusText = status ? "تایید" : "رد";
                TempData["Success"] = $"نظر با موفقیت {statusText} شد.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "خطایی در به‌روزرسانی وضعیت نظر رخ داد.";
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
                    await _mediator.Send(new UpdateReviewStatusCommand
                    {
                        ReviewId = reviewId,
                        Status = status
                    });
                }

                var statusText = status ? "تایید" : "رد";
                TempData["Success"] = $"{reviewIds.Length} نظر با موفقیت {statusText} شد.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "خطایی در به‌روزرسانی دسته‌ای نظرات رخ داد.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleShowHomePage(int reviewId, bool isShowHomePage)
        {
            try
            {
                await _mediator.Send(new ToggleReviewShowHomePageCommand
                {
                    ReviewId = reviewId,
                    IsShowHomePage = isShowHomePage
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
