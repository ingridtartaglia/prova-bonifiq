using ProvaPub.Models;

namespace ProvaPub.Services
{ 
    public abstract class PaginationService<T> where T : class
    {
        public PagedList<T> GetPagedList(IQueryable<T> query, int page)
        {
            int pageSize = 10;
            var skip = (page - 1) * pageSize;
            var totalCount = query.Count();

            var items = query.Skip(skip).Take(pageSize).ToList();

            var hasNext = totalCount > (page * pageSize);

            return new PagedList<T>() { HasNext = hasNext, TotalCount = totalCount, Items = items };
        }
    }
}
