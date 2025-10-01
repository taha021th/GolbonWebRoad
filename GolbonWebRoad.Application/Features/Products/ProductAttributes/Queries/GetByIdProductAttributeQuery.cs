
using GolbonWebRoad.Domain.Entities;
using MediatR;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using AutoMapper;
using GolbonWebRoad.Application.Exceptions;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Queries
{
    public class GetByIdProductAttributeQuery : IRequest<ProductAttribute>
    {
        public int Id { get; set; }
    }

    public class GetProductAttributeByIdQueryHandler : IRequestHandler<GetByIdProductAttributeQuery, ProductAttribute>
    {
        private readonly IProductAttributeRepository _productAttributeRepository;

        public GetProductAttributeByIdQueryHandler(IProductAttributeRepository productAttributeRepository)
        {
            _productAttributeRepository = productAttributeRepository;
        }

        public async Task<ProductAttribute> Handle(GetByIdProductAttributeQuery request, CancellationToken cancellationToken)
        {
            var attribute = await _productAttributeRepository.GetByIdAsync(request.Id);
            if (attribute == null)
            {
                throw new NotFoundException($"Product attribute with id {request.Id} not found");
            }
            return attribute;
        }
    }
}
