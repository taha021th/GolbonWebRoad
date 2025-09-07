//using GolbonWebRoad.Domain.Entities;
//using GolbonWebRoad.Infrastructure.Specifications.Base;

//namespace GolbonWebRoad.Infrastructure.Specifications
//{
//    public class ProductsWithFiltersSpecification : BaseSpecification<Product>
//    {
//        public ProductsWithFiltersSpecification(string? searchTerm, int? categoryId, bool joinCategory = false)
//            : base(p => string.IsNullOrEmpty(searchTerm) || p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) && (!categoryId.HasValue ||p.CategoryId==categoryId.Value)
//        {
//            if (joinCategory)
//            {
//                AddInclude(p => p.Category);
//            }
//        }
//    }
//}
