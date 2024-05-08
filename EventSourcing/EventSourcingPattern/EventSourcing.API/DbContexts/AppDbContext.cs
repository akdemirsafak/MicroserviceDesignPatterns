using EventSourcing.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.API.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}