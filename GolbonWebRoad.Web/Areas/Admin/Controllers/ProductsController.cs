using AutoMapper;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Features.Brands.Queries;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Application.Features.Products.Commands;
using GolbonWebRoad.Application.Features.Products.ProductAttributes.Queries;
using GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Queries;
using GolbonWebRoad.Application.Features.Products.ProductVariants.Commands;
using GolbonWebRoad.Application.Features.Products.ProductVariants.Queries;
using GolbonWebRoad.Application.Features.Products.Queries;
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
        private readonly IMapper _mapper;


        public ProductsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;

        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, string? searchTerm = null, int? categoryId = null, int? brandId = null, string? sortOrder = null)
        {
            var pagedProducts = await _mediator.Send(new GetPagedProductsQuery
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                BrandId = brandId,
                SortOrder = sortOrder
            });

            var productViewModels = _mapper.Map<GolbonWebRoad.Web.Models.Products.PagedResult<ProductViewModel>>(pagedProducts);
            return View(productViewModels);
        }
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateProductViewModel();
            await PopulateDropdownsAsync(viewModel);
            await PopulateAttributesAsync(viewModel);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(viewModel);
                await PopulateAttributesAsync(viewModel);
                return View(viewModel);
            }

            // Server-side pre-validation for variants (better UX before creating product)
            if (!await ValidateVariantsAsync(viewModel))
            {
                await PopulateDropdownsAsync(viewModel);
                await PopulateAttributesAsync(viewModel);
                return View(viewModel);
            }

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
                        Gtin=v.Gtin,
                        Height=v.Height,
                        Length=v.Length,
                        Mpn=v.Mpn,
                        Weight=v.Weight,
                        Width=v.Width,
                        AttributeValueIds = v.AttributeValueIds ?? new List<int>()
                    };
                    await _mediator.Send(createVariant);
                }
            }
            TempData["SuccessMessage"] = "محصول با موفقیت ایجاد شد.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id = id, JoinBrand = true, JoinCategory = true, JoinImages = true, JoinReviews = true });
            if (product == null) NotFound();
            var viewModel = _mapper.Map<EditProductViewModel>(product);
            // Load variants

            var variants = await _mediator.Send(new GetProductVariantByProductIdQuery { ProductId = id });
            viewModel.Variants = variants.Select(v => new VariantRowViewModel
            {
                Id = v.Id,
                Sku = v.Sku,
                Price = v.Price,
                OldPrice = v.OldPrice,
                StockQuantity = v.StockQuantity,
                Weight = v.Weight,
                Length = v.Length,
                Width = v.Width,
                Height = v.Height,
                Mpn=v.Mpn,
                Gtin=v.Gtin,
                AttributeValueIds = v.AttributeValues?.Select(av => av.Id).ToList() ?? new List<int>()
            }).ToList();
            await PopulateDropdownsAsync(viewModel);
            await PopulateAttributesAsync(viewModel);
            return View(viewModel);

        }

        // --- اکشن POST (اصلاح شده) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] EditProductViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(viewModel);
                await PopulateAttributesAsync(viewModel);
                // --- ۱. اصلاح اول: بارگذاری مجدد تصاویر در صورت خطای Model ---
                await RepopulateExistingImagesAsync(viewModel);
                return View(viewModel);
            }

            // Server-side pre-validation for variants
            if (!await ValidateVariantsAsync(viewModel))
            {
                await PopulateDropdownsAsync(viewModel);
                await PopulateAttributesAsync(viewModel);
                // --- ۲. اصلاح دوم: بارگذاری مجدد تصاویر در صورت خطای ولیدیشن ---
                await RepopulateExistingImagesAsync(viewModel);
                return View(viewModel);
            }
            try
            {
                var command = _mapper.Map<UpdateProductCommand>(viewModel);
                await _mediator.Send(command);

                // --- START: Variant Processing Logic ---
                if (viewModel.Variants != null && viewModel.Variants.Any())
                {
                    foreach (var variantVM in viewModel.Variants)
                    {
                        if (variantVM.MarkForDeletion && variantVM.Id.HasValue)
                        {
                            // 3. Delete Variant
                            await _mediator.Send(new DeleteProductVariantCommand { Id = variantVM.Id.Value });
                        }
                        else if (!variantVM.Id.HasValue)
                        {
                            // 1. Add New Variant
                            var createVariantCmd = new CreateProductVariantCommand
                            {
                                ProductId = viewModel.Id,
                                Sku = variantVM.Sku,
                                Price = variantVM.Price,
                                OldPrice = variantVM.OldPrice,
                                StockQuantity = variantVM.StockQuantity,
                                Gtin = variantVM.Gtin,
                                Mpn = variantVM.Mpn,
                                Weight = variantVM.Weight,
                                Length = variantVM.Length,
                                Width = variantVM.Width,
                                Height = variantVM.Height,
                                AttributeValueIds = variantVM.AttributeValueIds ?? new List<int>()
                            };
                            await _mediator.Send(createVariantCmd);
                        }
                        else if (variantVM.Id.HasValue)
                        {
                            // 2. Update Existing Variant
                            var updateVariantCmd = new UpdateProductVariantCommand
                            {
                                Id = variantVM.Id.Value,
                                Sku = variantVM.Sku,
                                Price = variantVM.Price,
                                OldPrice = variantVM.OldPrice,
                                StockQuantity = variantVM.StockQuantity,
                                Gtin = variantVM.Gtin,
                                Mpn = variantVM.Mpn,
                                Weight = variantVM.Weight,
                                Length = variantVM.Length,
                                Width = variantVM.Width,
                                Height = variantVM.Height,

                                AttributeValueIds = variantVM.AttributeValueIds ?? new List<int>()
                            };
                            await _mediator.Send(updateVariantCmd);
                        }
                    }
                }
                // --- END: Variant Processing Logic ---

                TempData["SuccessMessage"] = "محصول با موفقیت ویرایش شد";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "خطا در ویرایش محصول با شناسه {ProductId}", id);
                ModelState.AddModelError(string.Empty, "خطایی در هنگام ویرایش محصول رخ داد: " + ex.Message);
                await PopulateDropdownsAsync(viewModel);
                await PopulateAttributesAsync(viewModel); // <-- این خط هم در کد شما در بلاک catch جا افتاده بود

                // --- ۳. اصلاح سوم: بارگذاری مجدد تصاویر در صورت بروز Exception ---
                await RepopulateExistingImagesAsync(viewModel);
                return View(viewModel);
            }
        }

        // ... (متدهای Delete و Create بدون تغییر) ...


        // --- ۴. متد کمکی جدید ---
        /// <summary>
        /// این متد لیست تصاویر موجود محصول را (که در POST برنمی‌گردند)
        /// دوباره از دیتابیس می‌خواند و به ViewModel اضافه می‌کند.
        /// </summary>
        private async Task RepopulateExistingImagesAsync(EditProductViewModel viewModel)
        {
            try
            {
                // فقط تصاویر را از دیتابیس می‌خوانیم
                var productDto = await _mediator.Send(new GetProductByIdQuery
                {
                    Id = viewModel.Id,
                    AsNoTracking=true,
                    JoinImages = true
                });

                if (productDto != null)
                {
                    // خطا در اینجا بود:
                    // viewModel.Images = productDto.Images; 
                    viewModel.CurrentMainImageUrl=productDto.MainImageUrl;
                    // --- کد اصلاح شده ---
                    // ما باید لیست موجودیت‌ها را به لیست ویومدل‌ها مپ کنیم
                    viewModel.Images = _mapper.Map<List<ProductImageViewModel>>(productDto.Images);
                }
            }
            catch
            {
                viewModel.Images = new List<ProductImageViewModel>();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {

            var product = await _mediator.Send(new GetProductByIdQuery { Id = id, JoinCategory = true, JoinImages = true });
            if (product == null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<DeleteProductViewModel>(product);
            viewModel.ImageUrl = product.Images?.FirstOrDefault(i => i.IsMainImage)?.ImageUrl ?? product.Images?.FirstOrDefault()?.ImageUrl;

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

            viewModel.CategoryOptions = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
            viewModel.BrandOptions = new SelectList(brands, "Id", "Name", viewModel.BrandId);
        }
        private async Task PopulateDropdownsAsync(EditProductViewModel viewModel)
        {
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var brands = await _mediator.Send(new GetBrandsQuery());

            viewModel.CategoryOptions = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
            viewModel.BrandOptions = new SelectList(brands, "Id", "Name", viewModel.BrandId);
        }

        private async Task PopulateAttributesAsync(EditProductViewModel viewModel)
        {
            var allAttributes = await _mediator.Send(new GetAllProductAttributeQuery());
            var allValues = await _mediator.Send(new GetAllProductValueQuery());

            var productVariantIds = viewModel.Variants?.SelectMany(v => v.AttributeValueIds).ToHashSet() ?? new HashSet<int>();
            var usedAttributeIds = allValues.Where(v => productVariantIds.Contains(v.Id)).Select(v => v.AttributeId).ToHashSet();

            viewModel.AvailableAttributes = allAttributes.Select(a => new AttributeSelectionViewModel
            {
                Id = a.Id,
                Name = a.Name,
                IsSelected = usedAttributeIds.Contains(a.Id)
            }).ToList();

            var attrIdToName = allAttributes.ToDictionary(a => a.Id, a => a.Name);
            viewModel.AttributeGroups = allValues
                .GroupBy(v => v.AttributeId)
                .Select(g => new AttributeGroupOptionViewModel
                {
                    AttributeId = g.Key,
                    AttributeName = attrIdToName.ContainsKey(g.Key) ? attrIdToName[g.Key] : $"Attribute {g.Key}",
                    Values = g.Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Value }).ToList()
                })
                .OrderBy(gr => gr.AttributeName)
                .ToList();
        }

        private async Task PopulateAttributesAsync(CreateProductViewModel viewModel)
        {
            var allAttributes = await _mediator.Send(new GetAllProductAttributeQuery());
            var allValues = await _mediator.Send(new GetAllProductValueQuery());

            // For new products, we just show all attributes as unselected initially.
            viewModel.AvailableAttributes = allAttributes.Select(a => new AttributeSelectionViewModel
            {
                Id = a.Id,
                Name = a.Name,
                IsSelected = false
            }).ToList();

            var attrIdToName = allAttributes.ToDictionary(a => a.Id, a => a.Name);
            viewModel.AttributeGroups = allValues
                .GroupBy(v => v.AttributeId)
                .Select(g => new AttributeGroupOptionViewModel
                {
                    AttributeId = g.Key,
                    AttributeName = attrIdToName.ContainsKey(g.Key) ? attrIdToName[g.Key] : $"Attribute {g.Key}",
                    Values = g.Select(v => new SelectListItem { Value = v.Id.ToString(), Text = v.Value }).ToList()
                })
                .OrderBy(gr => gr.AttributeName)
                .ToList();
        }

        // --- Validation helpers ---
        private async Task<bool> ValidateVariantsAsync(CreateProductViewModel viewModel)
        {
            if (viewModel.Variants == null || !viewModel.Variants.Any()) return true;

            var valuesPaged = await _mediator.Send(new GetAllProductValueQuery());
            var idToAttr = valuesPaged.ToDictionary(v => v.Id, v => v.AttributeId);

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

            var valuesPaged = await _mediator.Send(new GetAllProductValueQuery());
            var idToAttr = valuesPaged.ToDictionary(v => v.Id, v => v.AttributeId);

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
