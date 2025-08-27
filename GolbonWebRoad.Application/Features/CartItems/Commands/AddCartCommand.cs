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
        public int Quantity { get; set; }
    }
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddToCartCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);
            if (product == null) throw new NotFoundException("محصول یافت نشد."); // Or a custom exception

            var cartItem = await _unitOfWork.CartItemRepository.GetCartItemAsync(request.UserId, request.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity += request.Quantity;
                _unitOfWork.CartItemRepository.UpdateCartItem(cartItem);
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await _unitOfWork.CartItemRepository.AddCartItemAsync(cartItem);
            }

            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
