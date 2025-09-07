using AutoMapper;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {

        private readonly IMediator _mediator;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        public ProductsController(IMediator mediator, IFileStorageService fileStorageService, IMapper mapper)
        {
            _mediator = mediator;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _mediator.Send(new GetProductsQuery());
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var viewModel = new CreateProductViewModel
            {

                Categories=_mapper.Map<IEnumerable<CategorySummaryDto>>(categories)
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _mediator.Send(new GetCategoriesQuery());
                model.Categories = _mapper.Map<IEnumerable<CategorySummaryDto>>(categories);
                return View(model);
            }

            var command = _mapper.Map<CreateProductCommand>(model);
            if (model.ImageFile != null)
            {
                // استفاده از سرویس برای ذخیره فایل
                command.ImageUrl = await _fileStorageService.SaveFileAsync(model.ImageFile, "images/products");
            }
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));


        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id=id });
            var categories = await _mediator.Send(new GetCategoriesQuery());

            if (product==null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<EditProductViewModel>(product);
            viewModel.Categories=_mapper.Map<IEnumerable<CategorySummaryDto>>(categories);
            viewModel.ExistingImageUrl=product.ImageUrl;
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var categories = await _mediator.Send(new GetCategoriesQuery());

                model.Categories=_mapper.Map<IEnumerable<CategorySummaryDto>>(categories);
                return View(model);
            }
            var command = _mapper.Map<UpdateProductCommand>(model);
            if (model.ImageFile!=null)
            {
                if (!string.IsNullOrEmpty(model.ExistingImageUrl))
                {
                    await _fileStorageService.DeleteFileAsync(Path.GetFileName(model.ExistingImageUrl), "images/products");
                }
                command.ImageUrl=await _fileStorageService.SaveFileAsync(model.ImageFile, "images/products");
            }
            else
            {
                command.ImageUrl=model.ExistingImageUrl;
            }
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int id)
        {

            var productDto = await _mediator.Send(new GetProductByIdQuery { Id=id, JoinCategory=true });
            if (productDto==null) return NotFound();
            var viewModel = _mapper.Map<DeleteProductViewModel>(productDto);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? imageUrl) // ✅ پارامتر جدید
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                await _fileStorageService.DeleteFileAsync(Path.GetFileName(imageUrl), "images/products");
            }

            await _mediator.Send(new DeleteProductCommand { Id = id });
            return RedirectToAction(nameof(Index));
        }


    }
}
