using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductVariants.Queries
{
    public class GetProductVariantByProductIdQuery : IRequest<ICollection<ProductVariant>>
    {
        public int ProductId { get; set; }

    }
    public class GetProductVariantByProductIdQueryHandler : IRequestHandler<GetProductVariantByProductIdQuery, ICollection<ProductVariant>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetProductVariantByProductIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<ICollection<ProductVariant>> Handle(GetProductVariantByProductIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductVariantRepository.GetByProductIdAsync(request.ProductId);

        }
    }
}
