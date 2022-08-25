using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Repository.Interface;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GeekShopping.CartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;

        public CouponRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<CouponVO> GetCouponByCouponCode(string code, string token)
        {
            // "api/v1/Coupon"

            token = token.Remove(0,7);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"/api/v1/Coupon/{code}");
            var content = await response.Content.ReadAsStringAsync();

            if(response.StatusCode != HttpStatusCode.OK)
            {
                return new CouponVO();
            }

            return JsonSerializer.Deserialize<CouponVO>(content,
                 new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
