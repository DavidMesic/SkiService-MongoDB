using Microsoft.EntityFrameworkCore;

namespace SkiServiceAPI.Models
{
    public class SkiServiceContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Auftrag> Aufträge { get; set; }



        public SkiServiceContext(DbContextOptions<SkiServiceContext> options)
            : base(options)
        {
        }
    }
}
