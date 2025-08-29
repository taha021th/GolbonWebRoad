using AutoMapper;
using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.CartItems.Queries
{
    public class GetCartQuery : IRequest<IEnumerable<CartItemDto>>
    {
        public string UserId { get; set; }
    }
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, IEnumerable<CartItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCartQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        public async Task<IEnumerable<CartItemDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cartItems = await _unitOfWork.CartItemRepository.GetCartItemsByUserIdAsync(request.UserId);
            return _mapper.Map<IEnumerable<CartItemDto>>(cartItems);
        }
    }
}