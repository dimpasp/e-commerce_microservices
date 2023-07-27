using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICardService
    {
        Task<ResponseDto?> GetCardByUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCardAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveFromCardAsync(int cardDetailsId);
        Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto);
        Task<ResponseDto?> EmailCart(CartDto cartDto);
    }
}
