using AutoMapper;
using GolbonWebRoad.Application.Dtos.Brands;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Features.Brands.Queries;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Products.ProductVariants.Commands;
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
            await PopulateAttributeValuesAsync(viewModel);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(viewModel);
                await PopulateAttributeValuesAsync(viewModel);
                return View(viewModel);
            }

            // Server-side pre-validation for variants (better UX before creating product)
            if (!await ValidateVariantsAsync(viewModel))
            {
                await PopulateDropdownsAsync(viewModel);
                await PopulateAttributeValuesAsync(viewModel);
                return View(viewModel);
            }

            //try
            //{
            var command = _mapper.Map<CreateProductCommand>(viewModel);
            var newProductId = await _mediator.Send(command);

            if (viewModel.Variants != null && viewModel.Variants.Any())
            {
                foreach (var v in viewModel.Variants)
                {
                    var createVariant = new CreateProductVariantCommand
                    {
                        ProductId = newProductId,
                        Sku = v.Sku,
                        Price = v.Price,
                        OldPrice = v.OldPrice,
                        StockQuantity = v.StockQuantity,
                        AttributeValueIds = v.AttributeValueIds ?? new List<int>()
                    };
                    await _mediator.Send(createVariant);
                }
            }
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
            var product = await _mediator.Send(new GetProductByIdQuery { Id=id, JoinBrand=true, JoinCategory =true, JoinImages=true, JoinReviews=true });
            if (product==null) NotFound();
            var viewModel = _mapper.Map<EditProductViewModel>(product);
            // Load variants
            var variants = await _unitOfWork.ProductVariantRepository.GetByProductIdAsync(id);
            viewModel.Variants = variants.Select(v => new VariantRowViewModel
            {
                Id = v.Id,
                Sku = v.Sku,
                Price = v.Price,
                OldPrice = v.OldPrice,
                StockQuantity = v.StockQuantity,
                AttributeValueIds = v.AttributeValues?.Select(av => av.Id).ToList() ?? new List<int>()
            }).ToList();
            await PopulateDropdownsAsync(viewModel);
            await PopulateAttributeValuesAsync(viewModel);
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
                await PopulateAttributeValuesAsync(viewModel);
                return View(viewModel);
            }

            // Server-side pre-validation for variants
            if (!await ValidateVariantsAsync(viewModel))
            {
                await PopulateDropdownsAsync(viewModel);
                await PopulateAttributeValuesAsync(viewModel);
                return View(viewModel);
            }
            try
            {
                var command = _mapper.Map<UpdateProductCommand>(viewModel);
                await _mediator.Send(command);

                // Handle variants
                if (viewModel.Variants != null)
                {
                    foreach (var vr in viewModel.Variants)
                    {
                        if (vr.Id.HasValue && vr.MarkForDeletion)
                        {
                            await _mediator.Send(new DeleteProductVariantCommand { Id = vr.Id.Value });
                            continue;
                        }

                        if (vr.Id.HasValue)
                        {
                            await _mediator.Send(new UpdateProductVariantCommand
                            {
                                Id = vr.Id.Value,
                                Sku = vr.Sku,
                                Price = vr.Price,
                                OldPrice = vr.OldPrice,
                                StockQuantity = vr.StockQuantity,
                                AttributeValueIds = vr.AttributeValueIds ?? new List<int>()
                            });
                        }
                        else
                        {
                            await _mediator.Send(new CreateProductVariantCommand
                            {
                                ProductId = id,
                                Sku = vr.Sku,
                                Price = vr.Price,
                                OldPrice = vr.OldPrice,
                                StockQuantity = vr.StockQuantity,
                                AttributeValueIds = vr.AttributeValueIds ?? new List<int>()
                            });
                        }
                    }
                }
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

        private async Task PopulateAttributeValuesAsync(EditProductViewModel viewModel)
        {
            var valuesPaged = await _unitOfWork.ProductAttributeValueRepository.GetAllAsync(1, int.MaxValue);
            var items = valuesPaged.Items
                .Select(v => new { v.Id, Text = v.Value })
                .ToList();
            viewModel.AttributeValueOptions = new SelectList(items, "Id", "Text");

            var attributesPaged = await _unitOfWork.ProductAttributeRepository.GetAllAsync(1, int.MaxValue);
            var attrIdToName = attributesPaged.Items.ToDictionary(a => a.Id, a => a.Name);
            var groups = valuesPaged.Items
                .GroupBy(v => v.AttributeId)
                .Select(g => new AttributeGroupOptionViewModel
                {
                    AttributeId = g.Key,
                    AttributeName = attrIdToName.ContainsKey(g.Key) ? attrIdToName[g.Key] : $"Attribute {g.Key}",
                    Values = g.Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Value }).ToList()
                })
                .OrderBy(gr => gr.AttributeName)
                .ToList();
            viewModel.AttributeGroups = groups;
        }

        private async Task PopulateAttributeValuesAsync(CreateProductViewModel viewModel)
        {
            // Load all attribute values flat for now; can group by attribute on UI if needed
            var attributesPaged = await _unitOfWork.ProductAttributeRepository.GetAllAsync(1, int.MaxValue);
            var valuesPaged = await _unitOfWork.ProductAttributeValueRepository.GetAllAsync(1, int.MaxValue);
            var items = valuesPaged.Items
                .Select(v => new { v.Id, Text = v.Value })
                .ToList();
            viewModel.AttributeValueOptions = new SelectList(items, "Id", "Text");

            var attrIdToName = attributesPaged.Items.ToDictionary(a => a.Id, a => a.Name);
            var groups = valuesPaged.Items
                .GroupBy(v => v.AttributeId)
                .Select(g => new AttributeGroupOptionViewModel
                {
                    AttributeId = g.Key,
                    AttributeName = attrIdToName.ContainsKey(g.Key) ? attrIdToName[g.Key] : $"Attribute {g.Key}",
                    Values = g.Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Value }).ToList()
                })
                .OrderBy(gr => gr.AttributeName)
                .ToList();
            viewModel.AttributeGroups = groups;
        }

        // --- Validation helpers ---
        private async Task<bool> ValidateVariantsAsync(CreateProductViewModel viewModel)
        {
            if (viewModel.Variants == null || !viewModel.Variants.Any()) return true;

            var valuesPaged = await _unitOfWork.ProductAttributeValueRepository.GetAllAsync(1, int.MaxValue);
            var idToAttr = valuesPaged.Items.ToDictionary(v => v.Id, v => v.AttributeId);

            var isValid = true;
            for (int i = 0; i < viewModel.Variants.Count; i++)
            {
                var v = viewModel.Variants[i];
                if (v == null) continue;

                // ensure selected IDs exist
                if (v.AttributeValueIds != null && v.AttributeValueIds.Any())
                {
                    foreach (var id in v.AttributeValueIds)
                    {
                        if (!idToAttr.ContainsKey(id))
                        {
                            ModelState.AddModelError($"Variants[{i}].AttributeValueIds", "مقدار ویژگی نامعتبر انتخاب شده است.");
                            isValid = false;
                        }
                    }

                    // ensure one per attribute
                    var attrDistinctCount = v.AttributeValueIds.Select(id => idToAttr.ContainsKey(id) ? idToAttr[id] : -1).Distinct().Count();
                    if (attrDistinctCount != v.AttributeValueIds.Count)
                    {
                        ModelState.AddModelError($"Variants[{i}].AttributeValueIds", "برای هر ویژگی فقط یک مقدار می‌توان انتخاب کرد.");
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        private async Task<bool> ValidateVariantsAsync(EditProductViewModel viewModel)
        {
            if (viewModel.Variants == null || !viewModel.Variants.Any()) return true;

            var valuesPaged = await _unitOfWork.ProductAttributeValueRepository.GetAllAsync(1, int.MaxValue);
            var idToAttr = valuesPaged.Items.ToDictionary(v => v.Id, v => v.AttributeId);

            var isValid = true;
            for (int i = 0; i < viewModel.Variants.Count; i++)
            {
                var v = viewModel.Variants[i];
                if (v == null || v.MarkForDeletion) continue;

                if (v.AttributeValueIds != null && v.AttributeValueIds.Any())
                {
                    foreach (var id in v.AttributeValueIds)
                    {
                        if (!idToAttr.ContainsKey(id))
                        {
                            ModelState.AddModelError($"Variants[{i}].AttributeValueIds", "مقدار ویژگی نامعتبر انتخاب شده است.");
                            isValid = false;
                        }
                    }

                    var attrDistinctCount = v.AttributeValueIds.Select(id => idToAttr.ContainsKey(id) ? idToAttr[id] : -1).Distinct().Count();
                    if (attrDistinctCount != v.AttributeValueIds.Count)
                    {
                        ModelState.AddModelError($"Variants[{i}].AttributeValueIds", "برای هر ویژگی فقط یک مقدار می‌توان انتخاب کرد.");
                        isValid = false;
                    }
                }
            }

            return isValid;
        }



    }
}
