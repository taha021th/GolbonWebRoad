using AutoMapper;
using GolbonWebRoad.Api.CacheRevalidations;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Features.Categories.Commands;
using GolbonWebRoad.Application.Features.Categories.Queries;
using GolbonWebRoad.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Security.Claims; // ۲. اضافه کردن using برای دسترسی به اطلاعات کاربر

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryController> _logger; // ۳. اطمینان از تزریق ILogger
        private readonly IMediator _mediator;

        public CategoryController(IMemoryCache cache, IMapper mapper, ILogger<CategoryController> logger, IMediator mediator)
        {
            _cache = cache;
            _mapper = mapper;
            _logger = logger;
            _mediator= mediator;
        }

        private string GetAdminId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetAll(bool? joinProducts = false)
        {
            _logger.LogInformation("درخواست برای دریافت لیست دسته‌بندی‌ها دریافت شد. JoinProducts: {JoinProducts}", joinProducts);

            string cacheKey = $"categories-{joinProducts}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<CategoryDto> cachedCategories))
            {
                _logger.LogInformation("پاسخ برای لیست دسته‌بندی‌ها از کش بازگردانده شد. کلید کش: {CacheKey}", cacheKey);
                return Ok(cachedCategories);
            }

            _logger.LogInformation("داده‌ای در کش یافت نشد، در حال ارسال کوئری به دیتابیس...");
            var categories = await _mediator.Send(new GetCategoriesQuery { JoinProducts = joinProducts });

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60));

            // ✅ بهبود مهم: استفاده از توکن صحیح برای کش دسته‌بندی‌ها
            cacheEntryOptions.AddExpirationToken(new CancellationChangeToken(CacheRevalidation.CategoryTokenSource.Token));

            _cache.Set(cacheKey, categories, cacheEntryOptions);

            _logger.LogInformation("تعداد {CategoryCount} دسته‌بندی از دیتابیس دریافت و در کش ذخیره شد.", categories.Count());
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, bool? joinProducts = false)
        {
            _logger.LogInformation("درخواست برای دریافت دسته‌بندی با شناسه {CategoryId} دریافت شد.", id);

            var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id, JoinProducts = joinProducts });
            if (category == null)
            {
                _logger.LogWarning("دسته‌بندی با شناسه {CategoryId} یافت نشد.", id);
                return NotFound();
            }

            _logger.LogInformation("دسته‌بندی با شناسه {CategoryId} و نام '{CategoryName}' با موفقیت یافت و ارسال شد.", id, category.Name);
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDto request)
        {
            var adminId = GetAdminId();
            _logger.LogInformation("ادمین {AdminId} درخواست ایجاد دسته‌بندی جدید با نام '{CategoryName}' را ارسال کرد.", adminId, request.Name);

            var command = _mapper.Map<CreateCategoryCommand>(request);
            var category = await _mediator.Send(command);

            _logger.LogInformation("باطل‌سازی کش محصولات و دسته‌بندی‌ها به دلیل ایجاد دسته‌بندی جدید.");
            CacheRevalidation.RevalidateProductAndCategoryCache();

            _logger.LogInformation("دسته‌بندی با شناسه {CategoryId} توسط ادمین {AdminId} با موفقیت ایجاد شد.", category.Id, adminId);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequestDto request)
        {
            var adminId = GetAdminId();
            _logger.LogInformation("ادمین {AdminId} درخواست به‌روزرسانی دسته‌بندی با شناسه {CategoryId} را ارسال کرد.", adminId, id);

            // ✅ بهبود مهم: Command باید Id را از route بگیرد، نه از بدنه درخواست
            var command = _mapper.Map<UpdateCategoryCommand>(request);
            command.Id = id;

            await _mediator.Send(command);

            _logger.LogInformation("باطل‌سازی کش محصولات و دسته‌بندی‌ها به دلیل به‌روزرسانی دسته‌بندی {CategoryId}.", id);
            CacheRevalidation.RevalidateProductAndCategoryCache();

            _logger.LogInformation("دسته‌بندی با شناسه {CategoryId} توسط ادمین {AdminId} با موفقیت به‌روزرسانی شد.", id, adminId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var adminId = GetAdminId();
            _logger.LogInformation("ادمین {AdminId} درخواست حذف دسته‌بندی با شناسه {CategoryId} را ارسال کرد.", adminId, id);

            await _mediator.Send(new DeleteCategoryCommand { Id = id });

            _logger.LogInformation("باطل‌سازی کش محصولات و دسته‌بندی‌ها به دلیل حذف دسته‌بندی {CategoryId}.", id);
            CacheRevalidation.RevalidateProductAndCategoryCache();

            _logger.LogInformation("دسته‌بندی با شناسه {CategoryId} توسط ادمین {AdminId} با موفقیت حذف شد.", id, adminId);
            return NoContent();
        }
    }
}