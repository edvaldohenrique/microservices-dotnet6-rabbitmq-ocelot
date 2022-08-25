using GeekShopping.Email.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShooping.Email.Model.Context
{
    public class MySqlContext : DbContext
    {
        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options) { } 

        public DbSet<EmailLog> Emails { get; set; }

    }
}
