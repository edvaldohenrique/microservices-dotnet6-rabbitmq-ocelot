using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;

        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
        }

        [HttpGet("{code}")]
        [Authorize]
        public async Task<ActionResult<CouponVO>> GetCouponByCouponCode(string code)
        {
            var coupon = await _couponRepository.GetCouponByCouponCode(code);

            if (coupon == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(coupon);
            }
        }
    }
}
