using GolbonWebRoad.Application.Features.Products.ProductAttributes.Commands;
using GolbonWebRoad.Application.Features.Products.ProductAttributes.Queries;
using GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Commands;
using GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Queries;
using GolbonWebRoad.Web.Areas.Admin.Models.ProductAttributeValue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductAttributeValueController : Controller
    {
        private readonly IMediator _mediator;

        public ProductAttributeValueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Admin/ProductAttributeValue?pageNumber=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> Index(int? attributeId, int pageNumber = 1, int pageSize = 20)
        {
            var attributes = await _mediator.Send(new GetAllProductAttributeQuery());
            var attrMap = attributes.ToDictionary(a => a.Id, a => a.Name);

            var vm = new ProductAttributeValueIndexViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                AttributeId = attributeId,
                AttributeName = attributeId.HasValue && attrMap.ContainsKey(attributeId.Value) ? attrMap[attributeId.Value] : null
            };

            if (attributeId.HasValue)
            {
                var pagedByAttr = await _mediator.Send(new GetProductValuesByAttributeIdPagedQuery { AttributeId = attributeId.Value, PageNumber = pageNumber, PageSize = pageSize });
                vm.TotalCount = pagedByAttr.TotalCount;
                vm.Items = pagedByAttr.Items?.Select(v => new ProductAttributeValueRowViewModel
                {
                    Id = v.Id,
                    AttributeId = v.AttributeId,
                    AttributeName = vm.AttributeName ?? (attrMap.ContainsKey(v.AttributeId) ? attrMap[v.AttributeId] : $"Attribute {v.AttributeId}"),
                    Value = v.Value
                }).ToList() ?? new List<ProductAttributeValueRowViewModel>();
            }
            else
            {
                var paged = await _mediator.Send(new GetAllProductValueByPagedQuery { PageNumber = pageNumber, PageSize = pageSize });
                vm.TotalCount = paged.TotalCount;
                vm.Items = paged.Items?.Select(v => new ProductAttributeValueRowViewModel
                {
                    Id = v.Id,
                    AttributeId = v.AttributeId,
                    AttributeName = attrMap.ContainsKey(v.AttributeId) ? attrMap[v.AttributeId] : $"Attribute {v.AttributeId}",
                    Value = v.Value
                }).ToList() ?? new List<ProductAttributeValueRowViewModel>();
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? attributeId)
        {
            var attributes = await _mediator.Send(new GetAllProductAttributeQuery());
            var vm = new CreateProductAttributeValueViewModel
            {
                AttributeId = attributeId ?? 0,
                AttributeOptions = new SelectList(attributes, "Id", "Name", attributeId)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductAttributeValueViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var attributes = await _mediator.Send(new GetAllProductAttributeQuery());
                viewModel.AttributeOptions = new SelectList(attributes, "Id", "Name", viewModel.AttributeId);
                return View(viewModel);
            }

            await _mediator.Send(new CreateProductAttributeValueCommand { AttributeId = viewModel.AttributeId, Value = viewModel.Value });
            return RedirectToAction(nameof(Index), new { attributeId = viewModel.AttributeId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var valueEntity = await _mediator.Send(new GetProductAttributeValueByIdQuery { Id = id });
            if (valueEntity == null) return NotFound();

            var attributes = await _mediator.Send(new GetAllProductAttributeQuery());
            var vm = new UpdateProductAttributeValueViewModel
            {
                Id = valueEntity.Id,
                AttributeId = valueEntity.AttributeId,
                Value = valueEntity.Value,
                AttributeOptions = new SelectList(attributes, "Id", "Name", valueEntity.AttributeId)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProductAttributeValueViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var attributes = await _mediator.Send(new GetAllProductAttributeQuery());
                viewModel.AttributeOptions = new SelectList(attributes, "Id", "Name", viewModel.AttributeId);
                return View(viewModel);
            }

            await _mediator.Send(new EditProductValueCommand { AttributeId = viewModel.AttributeId, Id = viewModel.Id, Value = viewModel.Value });
            return RedirectToAction(nameof(Index), new { attributeId = viewModel.AttributeId });
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int id, int? attributeId)
        {
            await _mediator.Send(new DeleteProductValueCommand { Id = id });
            return RedirectToAction(nameof(Index), new { attributeId });
        }
    }
}
