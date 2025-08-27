using AutoMapper;
using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.CartItems.Queries
{
    public class GetCartQuery : IRequest<List<CartItemDto>>
    {
        public string UserId { get; set; }

    }
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, List<CartItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCartQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        public async Task<List<CartItemDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cartItems = await _unitOfWork.CartItemRepository.GetCartItemsByUserIdAsync(request.UserId);
            return _mapper.Map<List<CartItemDto>>(cartItems);
        }
    }
}
