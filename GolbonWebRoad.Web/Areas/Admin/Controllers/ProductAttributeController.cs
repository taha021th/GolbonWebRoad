using GolbonWebRoad.Application.Features.Products.ProductAttributes.Commands;
using GolbonWebRoad.Application.Features.Products.ProductAttributes.queries;
using GolbonWebRoad.Application.Features.Products.ProductAttributes.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.ProductAttribute;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductAttributeController : Controller
    {
        private readonly IMediator _mediator;

        public ProductAttributeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Admin/ProductAttribute?pageNumber=1&pageSize=20
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 20)
        {
            var paged = await _mediator.Send(new GetAllProductAttributeByPagedQuery { PageNumber = pageNumber, PageSize = pageSize });

            var vm = new ProductAttributeIndexViewModel
            {
                Items = paged.Items?.Select(a => new ProductAttributeViewModel { Id = a.Id, Name = a.Name }).ToList() ?? new List<ProductAttributeViewModel>(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductAttributeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _mediator.Send(new CreateProductAttributeCommand { Name = viewModel.Name });
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var attribute = await _mediator.Send(new GetByIdProductAttributeQuery { Id = id });
            if (attribute == null) return NotFound();

            var viewModel = new UpdateProductAttributeViewModel { Id = attribute.Id, Name = attribute.Name };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProductAttributeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _mediator.Send(new EditProductAttributeCommand { Id = viewModel.Id, Name = viewModel.Name });
            return RedirectToAction(nameof(Index));
        }

        // Simple delete (GET). If you prefer confirmation, add a GET Delete view and a POST DeleteConfirmed action
        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {
            await _mediator.Send(new DeleteProductAttributeCommand { Id = id });
            return RedirectToAction(nameof(Index));
        }
    }
}
