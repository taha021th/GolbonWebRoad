using AutoMapper;
using GolbonWebRoad.Application.Dtos.Brands;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Features.Brands.Queries;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Products.Queries;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Web.Areas.Admin.Models.Products.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolbonWebRoad.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {

        private readonly IMediator _mediator;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IMediator mediator, IFileStorageService fileStorageService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _mediator.Send(new GetProductsForAdminQuery { JoinBrand=true, JoinCategory=true, JoinImages=true });
            var productViewModels = _mapper.Map<IEnumerable<ProductViewModel>>(products);
            return View(productViewModels);
        }
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateProductViewModel();
            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }

            //try
            //{
            var command = _mapper.Map<CreateProductCommand>(viewModel);
            await _mediator.Send(command);
            TempData["SuccessMessage"] = "محصول با موفقیت ایجاد شد.";
            return RedirectToAction(nameof(Index));
            //}
            //catch (Exception ex)
            //{
            //    // _logger.LogError(ex, "خطا در ایجاد محصول."); 
            //    ModelState.AddModelError(string.Empty, "خطایی در هنگام ایجاد محصول رخ داد. لطفا دوباره تلاش کنید.");
            //    await PopulateDropdownsAsync(viewModel); // <- فراخوانی متد کمکی در صورت خطا
            //    return View(viewModel);
            //}
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id=id, JoinBrand=true, JoinCategory =true, JoinColors=true, JoinImages=true, JoinReviews=true });
            if (product==null) NotFound();
            var viewModel = _mapper.Map<EditProductViewModel>(product);
            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductViewModel viewModel)
        {
            if (id!=viewModel.Id)
                return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }
            try
            {
                var command = _mapper.Map<UpdateProductCommand>(viewModel);
                await _mediator.Send(command);
                TempData["SuccessMessage"]="محصول با موفقیت ویرایش شد";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "خطا در ویرایش محصول با شناسه {ProductId}", id);
                ModelState.AddModelError(string.Empty, "خطایی در هنگام ویرایش محصول رخ داد.");
                await PopulateDropdownsAsync(viewModel);
                return View(viewModel);
            }

        }

        public async Task<IActionResult> Delete(int id)
        {

            var productDto = await _mediator.Send(new GetProductByIdQuery { Id=id, JoinCategory=true, JoinImages=true });
            if (productDto==null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<DeleteProductViewModel>(productDto);
            viewModel.ImageUrl = productDto.Images?.FirstOrDefault(i => i.IsMainImage)?.ImageUrl ?? productDto.Images?.FirstOrDefault()?.ImageUrl;

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // ✅ پارامتر جدید
        {


            try
            {
                await _mediator.Send(new DeleteProductCommand { Id = id });
                TempData["SuccessMessage"] = "محصول با موفقیت حذف شد.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "خطا در هنگام حذف محصول با شناسه {ProductId}", id);
                // کاربر را به یک صفحه خطا یا به لیست محصولات با یک پیام خطا هدایت کن
                TempData["ErrorMessage"] = "در هنگام حذف محصول خطایی رخ داد. لطفا دوباره تلاش کنید.";
                return RedirectToAction(nameof(Index));
            }
        }


        private async Task PopulateDropdownsAsync(CreateProductViewModel viewModel)
        {
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var brands = await _mediator.Send(new GetBrandsQuery());

            viewModel.CategoryOptions = new SelectList(_mapper.Map<IEnumerable<CategorySummaryDto>>(categories), "Id", "Name", viewModel.CategoryId);
            viewModel.BrandOptions = new SelectList(_mapper.Map<IEnumerable<BrandDto>>(brands), "Id", "Name", viewModel.BrandId);
        }
        private async Task PopulateDropdownsAsync(EditProductViewModel viewModel)
        {
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var brands = await _mediator.Send(new GetBrandsQuery());

            viewModel.CategoryOptions = new SelectList(_mapper.Map<IEnumerable<CategorySummaryDto>>(categories), "Id", "Name", viewModel.CategoryId);
            viewModel.BrandOptions = new SelectList(_mapper.Map<IEnumerable<BrandDto>>(brands), "Id", "Name", viewModel.BrandId);
        }



    }
}
