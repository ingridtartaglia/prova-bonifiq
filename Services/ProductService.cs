using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class ProductService : PaginationService<Product>
    {
        private readonly TestDbContext _ctx;

		public ProductService(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public PagedList<Product> ListProducts(int page)
		{
            return GetPagedList(_ctx.Products, page);
        }

	}
}
