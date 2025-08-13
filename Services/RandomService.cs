using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class RandomService
	{
        private readonly DbContextOptions<TestDbContext> _ctx;

        public RandomService()
        {
            _ctx = new DbContextOptionsBuilder<TestDbContext>()
                        .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Teste;Trusted_Connection=True;")
                        .Options;
        }

        public async Task<int> GetRandom()
        {
            using (var ctx = new TestDbContext(_ctx))
            {
                var random = new Random();
                var generatedNumbers = new List<int>();

                for (int i = 0; i < 100; i++)
                {
                    generatedNumbers.Add(random.Next(1, 100));
                }

                var existentNumbers = await ctx.Numbers
                                            .Where(n => generatedNumbers.Contains(n.Number))
                                            .Select(n => n.Number)
                                            .ToListAsync();

                var number = generatedNumbers.FirstOrDefault(n => !existentNumbers.Contains(n));

                if (number == 0)
                {
                    throw new Exception("Failed to generate a unique number after 100 tries");
                }

                var newNumber = new RandomNumber { Number = number };
                ctx.Numbers.Add(newNumber);
                await ctx.SaveChangesAsync();

                return number;
            }
        }
    }
}
