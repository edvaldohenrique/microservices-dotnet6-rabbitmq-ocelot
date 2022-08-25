using GeekShooping.Email.Model.Context;
using GeekShopping.Email.Messages;
using GeekShopping.Email.Model;
using GeekShopping.Email.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<MySqlContext> _context;

        public EmailRepository(DbContextOptions<MySqlContext> context)
        {
            _context = context;
        }

        public async Task LogEmail(UpdatePaymentResultMessage update)
        {
            EmailLog email = new EmailLog()
            {
                Email = update.Email,
                SendDate = DateTime.Now,
                Log = $"Order - {update.OrderId} has been created successfully!!"
            };
            await using var _db = new MySqlContext(_context);
             _db.Emails.Add(email);

            await _db.SaveChangesAsync();
        }
    }
}
