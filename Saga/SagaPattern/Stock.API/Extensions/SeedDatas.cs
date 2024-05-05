using Stock.API.DbContexts;

namespace Stock.API.Extensions
{
    public static class SeedDatas
    {
        public static async void Handle<T>(AppDbContext _dbContext, IEnumerable<T> entities) where T : class
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }
    }
}
public class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        var stocks = new List<Stock.API.Models.Stock>{
            new(){
                Id=1,
                ProductId=1,
                Count=10
            },
            new(){
                Id=2,
                ProductId=2,
                Count=50
            },
            new(){
                Id=3,
                ProductId=5,
                Count=150
            },
        };
        Stock.API.Extensions.SeedDatas.Handle(context, stocks);

    }
}