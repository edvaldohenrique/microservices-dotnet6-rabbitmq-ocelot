using GeekShopping.Email.Messages;

namespace GeekShopping.Email.Repository.Interface
{
    public interface IEmailRepository
    {
        Task LogEmail(UpdatePaymentResultMessage update);
    }
}
