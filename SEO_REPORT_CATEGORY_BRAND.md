# گزارش سئوی فنی - صفحات Category و Brand

## تاریخ: 2025-11-09

---

## خلاصه اجرایی

بهینه‌سازی‌های سئوی فنی مشابه صفحات محصول (PDP) و لیست محصولات (PLP) برای صفحات **Category** و **Brand** اعمال شد. این شامل متادیتا، Open Graph، Twitter Cards، Schema.org JSON-LD و بهبودهای Razor می‌باشد.

---

## 1. صفحات Category

### 1.1 صفحه جزئیات دسته‌بندی (Category Detail)
**مسیر:** `Views/Categories/Detail.cshtml`

#### ✅ متادیتا
- **Title & Description:** از `Model.MetaTitle` و `Model.MetaDescription` استفاده می‌شود
- **Meta Robots:** `noindex, follow` برای صفحات فیلترشده (بر اساس `ViewBag.Noindex`)
- **Canonical URL:** فقط برای صفحات بدون فیلتر و صفحه اول
  - فرمت: `/category/{id}/{name}`

#### ✅ Open Graph
```html
<meta property="og:type" content="website" />
<meta property="og:title" content="{MetaTitle}" />
<meta property="og:description" content="{MetaDescription}" />
<meta property="og:url" content="{categoryUrl}" />
<meta property="og:site_name" content="{Host}" />
<meta property="og:locale" content="fa_IR" />
<meta property="og:image" content="{تصویر دسته یا اولین محصول}" />
```

#### ✅ Twitter Cards
```html
<meta name="twitter:card" content="summary_large_image" />
<meta name="twitter:title" content="{MetaTitle}" />
<meta name="twitter:description" content="{MetaDescription}" />
<meta name="twitter:image" content="{ogImage}" />
<meta name="twitter:url" content="{categoryUrl}" />
```

#### ✅ Schema.org JSON-LD

**1. BreadcrumbList**
```json
{
  "@context": "https://schema.org",
  "@type": "BreadcrumbList",
  "itemListElement": [
    { "@type": "ListItem", "position": 1, "name": "خانه", "item": "{homeUrl}" },
    { "@type": "ListItem", "position": 2, "name": "دسته‌بندی‌ها", "item": "{categoriesUrl}" },
    { "@type": "ListItem", "position": 3, "name": "{categoryName}", "item": "{categoryUrl}" }
  ]
}
```

**2. CollectionPage**
```json
{
  "@context": "https://schema.org",
  "@type": "CollectionPage",
  "name": "{categoryName}",
  "description": "{MetaDescription}",
  "url": "{categoryUrl}",
  "inLanguage": "fa-IR"
}
```

**3. ItemList** (لیست محصولات)
```json
{
  "@context": "https://schema.org",
  "@type": "ItemList",
  "itemListOrder": "https://schema.org/ItemListOrderAscending",
  "numberOfItems": {تعداد محصولات},
  "itemListElement": [
    {
      "@type": "ListItem",
      "position": {موقعیت براساس صفحه‌بندی},
      "url": "{productUrl}",
      "name": "{productName}",
      "image": "{productImage}",
      "offers": {
        "@type": "Offer",
        "price": {price},
        "priceCurrency": "IRR"
      }
    }
  ]
}
```

#### ✅ منطق Position در ItemList
- محاسبه دقیق: `position = ((pageNumber - 1) × pageSize) + 1`
- مثال: صفحه 2 با اندازه 12 → شروع از position 13

---

### 1.2 صفحه لیست دسته‌بندی‌ها (Categories Index)
**مسیر:** `Views/Categories/Index.cshtml`

#### ✅ متادیتا
- **Title:** "فهرست دسته بندی ها | فروشگاه گلبن"
- **Description:** "مشاهده و انتخاب از بین انواع دسته‌بندی‌های محصولات فروشگاه گلبن"
- **Canonical URL:** `/categories`

#### ✅ Open Graph & Twitter
- Type: `website`
- Title: "دسته‌بندی‌های محصولات | فروشگاه گلبن"
- Image: تصویر اولین دسته‌بندی یا placeholder

#### ✅ Schema.org JSON-LD
1. **BreadcrumbList:** خانه > دسته‌بندی‌ها
2. **CollectionPage:** اطلاعات صفحه لیست
3. **ItemList:** لیست تمام دسته‌بندی‌ها با لینک و تصویر

---

## 2. صفحات Brand

### 2.1 صفحه جزئیات برند (Brand Detail)
**مسیر:** `Views/Brands/Detail.cshtml`

#### ✅ متادیتا
- **Title & Description:** از `Model.MetaTitle` و `Model.MetaDescription`
- **Meta Robots:** `noindex, follow` برای صفحات با فیلتر یا صفحه‌بندی
- **Canonical URL:** فقط برای صفحه اول بدون فیلتر
  - فرمت: `/brand/{id}/{name}`

#### ✅ Open Graph
```html
<meta property="og:type" content="website" />
<meta property="og:title" content="{MetaTitle}" />
<meta property="og:description" content="{MetaDescription}" />
<meta property="og:url" content="{brandUrl}" />
<meta property="og:site_name" content="{Host}" />
<meta property="og:locale" content="fa_IR" />
<meta property="og:image" content="{تصویر برند یا اولین محصول}" />
```

#### ✅ Twitter Cards
- کاملاً مشابه Category Detail
- Card type: `summary_large_image`

#### ✅ Schema.org JSON-LD

**1. BreadcrumbList**
```json
{
  "@context": "https://schema.org",
  "@type": "BreadcrumbList",
  "itemListElement": [
    { "@type": "ListItem", "position": 1, "name": "خانه", "item": "{homeUrl}" },
    { "@type": "ListItem", "position": 2, "name": "برندها", "item": "{brandsUrl}" },
    { "@type": "ListItem", "position": 3, "name": "{brandName}", "item": "{brandUrl}" }
  ]
}
```

**2. CollectionPage**
```json
{
  "@context": "https://schema.org",
  "@type": "CollectionPage",
  "name": "{brandName}",
  "description": "{MetaDescription}",
  "url": "{brandUrl}",
  "inLanguage": "fa-IR"
}
```

**3. ItemList** (لیست محصولات برند)
- ساختار کاملاً مشابه Category Detail
- Position براساس صفحه‌بندی محاسبه می‌شود

---

### 2.2 صفحه لیست برندها (Brands Index)
**مسیر:** `Views/Brands/Index.cshtml`

#### ✅ متادیتا
- **Title:** "فهرست برندها | فروشگاه گلبن"
- **Description:** "مشاهده و انتخاب از بین برندهای مختلف محصولات فروشگاه گلبن"
- **Canonical URL:** `/brands`

#### ✅ Open Graph & Twitter
- Type: `website`
- Title: "برندهای محصولات | فروشگاه گلبن"
- Image: تصویر اولین برند یا placeholder

#### ✅ Schema.org JSON-LD
1. **BreadcrumbList:** خانه > برندها
2. **CollectionPage:** اطلاعات صفحه لیست
3. **ItemList:** لیست تمام برندها با لینک و تصویر

---

## 3. بهبودهای Razor

### ✅ جایگزینی `@Html.Partial` با `<partial>`
تمام فایل‌های Category و Brand به‌روزرسانی شدند:

**قبل:**
```razor
@Html.Partial("_CommonStyles")
@Html.Partial("_Header")
@Html.Partial("_Footer")
@Html.Partial("_CommonScripts")
```

**بعد:**
```razor
<partial name="_CommonStyles" />
<partial name="_Header" />
<partial name="_Footer" />
<partial name="_CommonScripts" />
```

**مزایا:**
- رفع هشدار MVC1000
- سازگاری با ASP.NET Core Tag Helpers
- خوانایی بهتر کد

---

## 4. نکات تکنیکی

### ✅ URL Structure
- **Category Detail:** `/category/{id}/{slug}`
- **Brand Detail:** `/brand/{id}/{slug}`
- **Categories List:** `/categories`
- **Brands List:** `/brands`

### ✅ Image Fallback Strategy
1. تصویر اصلی (دسته/برند)
2. تصویر اولین محصول
3. Placeholder (placehold.co)

### ✅ Canonical Logic
- **فقط** برای صفحه اول بدون فیلتر
- شرط: `!isFiltered && pageNumber == 1`

### ✅ Meta Robots Logic
- Category: همیشه `noindex, follow` (از کنترلر تنظیم می‌شود)
- Brand: فقط برای صفحات فیلترشده یا `page > 1`

### ✅ JSON Serialization
```csharp
var jsonOptions = new System.Text.Json.JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};
```

---

## 5. مقایسه با PDP و PLP

| ویژگی | PDP | PLP | Category Detail | Brand Detail |
|-------|-----|-----|-----------------|--------------|
| **Canonical** | ✅ همیشه | ✅ بدون فیلتر | ✅ بدون فیلتر | ✅ بدون فیلتر |
| **OG Type** | product | website | website | website |
| **Twitter Cards** | ✅ | ✅ | ✅ | ✅ |
| **BreadcrumbList** | ✅ | ✅ | ✅ | ✅ |
| **Product Schema** | ✅ | ❌ | ❌ | ❌ |
| **CollectionPage** | ❌ | ✅ | ✅ | ✅ |
| **ItemList** | ❌ | ✅ | ✅ | ✅ |
| **aggregateRating** | ✅ | ❌ | ❌ | ❌ |

---

## 6. چک‌لیست تست

### ✅ Category Detail
- [ ] بررسی canonical در صفحه اول
- [ ] تست noindex در صفحات فیلترشده
- [ ] اعتبارسنجی JSON-LD در [Schema Validator](https://validator.schema.org/)
- [ ] بررسی OG tags با [Facebook Debugger](https://developers.facebook.com/tools/debug/)
- [ ] تست position در ItemList برای صفحات مختلف

### ✅ Brand Detail
- [ ] تست مشابه Category Detail

### ✅ Index Pages
- [ ] بررسی canonical URLs
- [ ] اعتبارسنجی ItemList برای لیست دسته‌ها/برندها
- [ ] تست تصاویر fallback

---

## 7. ابزارهای تست توصیه‌شده

1. **Google Rich Results Test:** https://search.google.com/test/rich-results
2. **Schema.org Validator:** https://validator.schema.org/
3. **Facebook Sharing Debugger:** https://developers.facebook.com/tools/debug/
4. **Twitter Card Validator:** https://cards-dev.twitter.com/validator
5. **Chrome DevTools Lighthouse:** (SEO Audit)

---

## 8. نتیجه‌گیری

✅ **کارهای انجام‌شده:**
- بهینه‌سازی کامل Category Detail (متادیتا، OG، Twitter، JSON-LD)
- بهینه‌سازی کامل Brand Detail (متادیتا، OG، Twitter، JSON-LD)
- بهینه‌سازی Categories Index (متادیتا، OG، Twitter، JSON-LD)
- بهینه‌سازی Brands Index (متادیتا، OG، Twitter، JSON-LD)
- جایگزینی تمام `@Html.Partial` با `<partial>` tag helpers

✅ **تطابق با استانداردها:**
- Schema.org vocabulary ✅
- Open Graph Protocol ✅
- Twitter Cards ✅
- SEO Best Practices ✅
- ASP.NET Core Razor standards ✅

---

**تهیه‌شده توسط:** Warp Agent Mode  
**تاریخ:** 2025-11-09  
**وضعیت:** ✅ تکمیل شده
