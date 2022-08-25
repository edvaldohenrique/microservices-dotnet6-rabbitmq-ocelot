using AutoMapper;
using GeekShooping.CouponAPI.Model.Context;
using GeekShopping.CouponAPI.Data.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly MySqlContext _context;
        private readonly IMapper _mapper;

        public CouponRepository(MySqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CouponVO> GetCouponByCouponCode(string code)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync( c => 
            c.CouponCode==code);

            return _mapper.Map<CouponVO>(coupon);
        }
    }
}
