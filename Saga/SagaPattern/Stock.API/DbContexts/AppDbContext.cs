using Microsoft.EntityFrameworkCore;

namespace Stock.API.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Stock.API.Models.Stock> Stocks { get; set; }
    }
}