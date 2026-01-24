using GolbonWebRoad.Application.Features.Faqs.Queries;
using GolbonWebRoad.Web.Models.Faqs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Controllers
{
    public class FaqController : Controller
    {
        private readonly IMediator _mediator;
        public FaqController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var faqs = await _mediator.Send(new GetFaqsQuery { OnlyActive = true });
            var grouped = faqs
                .GroupBy(f => string.IsNullOrWhiteSpace(f.CategoryName) ? "سایر" : f.CategoryName!)
                .OrderBy(g => g.Key)
                .Select(g => new FaqCategoryViewModel
                {
                    Category = g.Key,
                    Items = g.OrderBy(i => i.SortOrder).ThenBy(i => i.Id)
                        .Select(i => new FaqItemViewModel
                        {
                            Id = i.Id,
                            Question = i.Question,
                            AnswerHtml = i.Answer,
                            Slog = i.Slog,
                            Tags = i.Tags
                        }).ToList()
                }).ToList();
            var vm = new FaqPageViewModel { Categories = grouped };
            return View(vm);
        }
    }
}
