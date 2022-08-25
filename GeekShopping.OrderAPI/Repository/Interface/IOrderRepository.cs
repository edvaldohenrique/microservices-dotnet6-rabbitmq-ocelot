using GeekShopping.OrderAPI.Model;

namespace GeekShopping.OrderAPI.Repository.Interface
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader header);
        Task UpdatePaymentStatus(long orderHeaderId,  bool status);
    }
}
