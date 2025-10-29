namespace GolbonWebRoad.Web.Models.Products
{
    public class HomeProductViewModel
    {
        public IEnumerable<ProductViewModel> Products { get; set; }
        public ProductViewModel ProductIsFeatured { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slog { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string ImageUrl { get; set; }
        public string? MainImageAltText { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
    }
    public class ProductIndexViewModel
    {
        public PagedResult<ProductViewModel> Products { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public List<BrandViewModel> Brands { get; set; }

        // SEO Fields
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }

        // To keep track of current filters
        public int? CurrentCategoryId { get; set; }
        public int? CurrentBrandId { get; set; }
        public string SearchTerm { get; set; }
        public string CurrentSortOrder { get; set; }
    }

    // Simple ViewModels for filter lists
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    public class BrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // A generic class for paged results to be used in views    
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // Add these two properties to fix the error
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
    public class ProductImagesViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMainImage { get; set; }

    }

    // Removed ProductColorViewModel (colors are deprecated)

    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }

    public class ReviewFormViewModel
    {
        public int ProductId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
    }

    public class ProductDetailViewModel
    {

        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string? MainImageUrl { get; set; }

        // SEO Fields
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? MainImageAltText { get; set; }


        public CategoryViewModel Category { get; set; }
        public BrandViewModel Brand { get; set; }
        public int ReviewsCount { get; set; }

        public List<ProductImagesViewModel> Images { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();

        // --- Variants & Attributes (for new structure) ---
        public List<VariantDisplayViewModel> Variants { get; set; } = new List<VariantDisplayViewModel>();
        public List<AttributeGroupDisplayViewModel> AttributeGroups { get; set; } = new List<AttributeGroupDisplayViewModel>();
    }
}

namespace GolbonWebRoad.Web.Models.Cart
{
    public class ProductCartViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ProductCartViewModel Product { get; set; }
        public Dictionary<string, string> VariantAttributes { get; set; } = new Dictionary<string, string>();
        public decimal TotalPrice => Quantity * Price;
    }
    public class CartViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal GrandTotal { get; set; }
    }
}
