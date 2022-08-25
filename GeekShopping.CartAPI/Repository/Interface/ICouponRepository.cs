using GeekShopping.CartAPI.Data.ValueObjects;

namespace GeekShopping.CartAPI.Repository.Interface
{
    public interface ICouponRepository
    {
        Task<CouponVO> GetCouponByCouponCode(string code, string token);
    }
}
