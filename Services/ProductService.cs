using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class ProductService
	{
		TestDbContext _ctx;

		public ProductService(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public PagedList<Product> ListProducts(int page)
		{
            int pageSize = 10;
            var skip = (page - 1) * pageSize;
            var totalCount = _ctx.Products.Count();

			var products = _ctx.Products.Skip(skip).Take(pageSize).ToList();

			var hasNext = totalCount > (page * pageSize);

            return new PagedList<Product>() { HasNext = hasNext, TotalCount = totalCount, Items = products };
		}

	}
}
