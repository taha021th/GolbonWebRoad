using AutoMapper;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.CartItems.Commands
{
    public class AddToCartCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }

    }
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddToCartCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);
            if (product == null) throw new NotFoundException("محصول یافت نشد."); // Or a custom exception

            var cartItem = await _unitOfWork.CartItemRepository.GetCartItemAsync(request.UserId, request.ProductId);


            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                _unitOfWork.CartItemRepository.UpdateCartItem(cartItem);
            }
            else
            {
                var entity = _mapper.Map<CartItem>(request);
                _unitOfWork.CartItemRepository.AddCartItem(entity);
            }

            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
