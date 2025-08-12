using AutoMapper;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Web.Areas.Admin.Models;
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
        private readonly IFileStorageService _fileStorageService; // جایگزینی IWebHostEnvironment
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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var command = _mapper.Map<CreateProductCommand>(model);
                if (model.ImageFile != null)
                {
                    // استفاده از سرویس برای ذخیره فایل
                    command.ImageUrl = await _fileStorageService.SaveFileAsync(model.ImageFile, "images/products");
                }
                await _mediator.Send(command);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id!=model.Id) return BadRequest();
            if (ModelState.IsValid)
            {
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
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {

            var productDto = await _mediator.Send(new GetProductByIdQuery { Id=id });
            if (productDto==null) return NotFound();
            return View(productDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var productToDelete = await _mediator.Send(new GetProductByIdQuery { Id=id });
            if (productToDelete !=null && !string.IsNullOrEmpty(productToDelete.ImageUrl))
            {
                await _fileStorageService.DeleteFileAsync(Path.GetFileName(productToDelete.ImageUrl), "images/products");
            }
            await _mediator.Send(new DeleteProductCommand { Id=id });
            return RedirectToAction(nameof(Index));

        }

    }
}
