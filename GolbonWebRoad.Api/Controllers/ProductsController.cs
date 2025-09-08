//using AutoMapper;
//using GolbonWebRoad.Api.CacheRevalidations;
//using GolbonWebRoad.Application.Dtos.Products;
//using GolbonWebRoad.Application.Features.Products.Commands;
//using GolbonWebRoad.Application.Features.Products.Queries;
//using GolbonWebRoad.Application.Interfaces.Services;
//using GolbonWebRoad.Domain.Enums;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Primitives;
//using System.Security.Claims;

//namespace GolbonWebRoad.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductsController : ControllerBase
//    {
//        private readonly IMemoryCache _cache;
//        private readonly IMapper _mapper;
//        private readonly IFileStorageService _fileStorageService;
//        private readonly ILogger<ProductsController> _logger;
//        private readonly IMediator _mediator;

//        public ProductsController(IMemoryCache cache, IMapper mapper, IFileStorageService fileStorageService, ILogger<ProductsController> logger, IMediator mediator)
//        {
//            _cache = cache;
//            _mapper = mapper;
//            _fileStorageService = fileStorageService;
//            _logger = logger;
//            _mediator=mediator;
//        }

//        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

//        [HttpGet]
//        public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery query)
//        {
//            _logger.LogInformation("درخواست برای دریافت لیست محصولات با فیلترها دریافت شد: Search='{SearchTerm}', CategoryId={CategoryId}, Sort='{SortOrder}'",
//                query.SearchTerm, query.CategoryId, query.SortOrder);

//            string cacheKey = $"products-{query.SearchTerm}-{query.CategoryId}-{query.SortOrder}-{query.JoinCategory}";

//            if (_cache.TryGetValue(cacheKey, out IEnumerable<ProductDto> cachedProducts))
//            {
//                _logger.LogInformation("پاسخ برای لیست محصولات از کش بازگردانده شد. کلید کش: {CacheKey}", cacheKey);
//                return Ok(cachedProducts);
//            }

//            _logger.LogInformation("داده‌ای در کش یافت نشد، در حال ارسال کوئری به دیتابیس...");
//            var products = await _mediator.Send(query);

//            var productCacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(24));
//            productCacheOptions.AddExpirationToken(new CancellationChangeToken(CacheRevalidation.ProductTokenSource.Token));
//            _cache.Set(cacheKey, products, productCacheOptions);

//            _logger.LogInformation("تعداد {ProductCount} محصول از دیتابیس دریافت و در کش ذخیره شد.", products.Count());
//            return Ok(products);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id, [FromQuery] bool joinCategory = false)
//        {
//            _logger.LogInformation("درخواست برای دریافت محصول با شناسه {ProductId} دریافت شد.", id);

//            var product = await _mediator.Send(new GetProductByIdQuery { Id = id, JoinCategory = joinCategory });
//            if (product == null)
//            {
//                _logger.LogWarning("محصول با شناسه {ProductId} یافت نشد.", id);
//                return NotFound(new { Message = $"Product with {id} not found." });
//            }

//            _logger.LogInformation("محصول با شناسه {ProductId} با نام '{ProductName}' با موفقیت یافت و ارسال شد.", id, product.Name);
//            return Ok(product);
//        }

//        [HttpPost]
//        [Authorize(Roles = AppRoles.Admin)]
//        public async Task<IActionResult> Create([FromForm] CreateProductRequestDto request)
//        {
//            var adminId = GetUserId();
//            _logger.LogInformation("ادمین {AdminId} درخواست ایجاد محصول جدید با نام '{ProductName}' را ارسال کرد.", adminId, request.Name);

//            var command = _mapper.Map<CreateProductCommand>(request);
//            if (request.ImageFile != null)
//            {
//                _logger.LogInformation("فایل تصویر برای محصول '{ProductName}' دریافت شد، در حال ذخیره‌سازی...", request.Name);
//                command.ImageUrl = await _fileStorageService.SaveFileAsync(request.ImageFile, "products");
//                _logger.LogInformation("فایل تصویر در مسیر '{ImageUrl}' ذخیره شد.", command.ImageUrl);
//            }

//            var product = await _mediator.Send(command);

//            _logger.LogInformation("باطل‌سازی کش محصولات و دسته‌بندی‌ها به دلیل ایجاد محصول جدید.");
//            CacheRevalidation.RevalidateProductAndCategoryCache();

//            _logger.LogInformation("محصول با شناسه {ProductId} توسط ادمین {AdminId} با موفقیت ایجاد شد.", product.Id, adminId);
//            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
//        }

//        [HttpPut("{id}")]
//        [Authorize(Roles = AppRoles.Admin)]
//        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductRequestDto request)
//        {
//            var adminId = GetUserId();
//            _logger.LogInformation("ادمین {AdminId} درخواست به‌روزرسانی محصول با شناسه {ProductId} را ارسال کرد.", adminId, id);

//            if (id != request.Id)
//            {

//                _logger.LogError("عدم تطابق شناسه! شناسه در URL ({UrlId}) با شناسه در بدنه درخواست ({BodyId}) برای ادمین {AdminId} متفاوت است.", id, request.Id, adminId);
//                return BadRequest("Id in URL does not match Id in body.");
//            }



//            var command = _mapper.Map<UpdateProductCommand>(request);
//            if (request.ImageFile != null)
//            {
//                _logger.LogInformation("فایل تصویر جدیدی برای محصول {ProductId} دریافت شد، در حال ذخیره‌سازی...", id);
//                command.ImageUrl = await _fileStorageService.SaveFileAsync(request.ImageFile, "products");
//            }

//            await _mediator.Send(command);

//            _logger.LogInformation("باطل‌سازی کش محصولات و دسته‌بندی‌ها به دلیل به‌روزرسانی محصول {ProductId}.", id);
//            CacheRevalidation.RevalidateProductAndCategoryCache();

//            _logger.LogInformation("محصول با شناسه {ProductId} توسط ادمین {AdminId} با موفقیت به‌روزرسانی شد.", id, adminId);
//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        [Authorize(Roles = AppRoles.Admin)]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var adminId = GetUserId();
//            _logger.LogInformation("ادمین {AdminId} درخواست حذف محصول با شناسه {ProductId} را ارسال کرد.", adminId, id);



//            await _mediator.Send(new DeleteProductCommand { Id = id });

//            _logger.LogInformation("باطل‌سازی کش محصولات و دسته‌بندی‌ها به دلیل حذف محصول {ProductId}.", id);
//            CacheRevalidation.RevalidateProductAndCategoryCache();

//            _logger.LogInformation("محصول با شناسه {ProductId} توسط ادمین {AdminId} با موفقیت حذف شد.", id, adminId);
//            return NoContent();
//        }
//    }
//}