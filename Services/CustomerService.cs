using Microsoft.EntityFrameworkCore;
using ProvaPub.DTOs;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class CustomerService : PaginationService<Customer>
    {
        private readonly TestDbContext _ctx;

        public CustomerService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public PagedList<CustomerDto> ListCustomers(int page)
        {
            var pagedCustomers = GetPagedList(_ctx.Customers.Include(c => c.Orders), page);

            var customerDtos = pagedCustomers.Items
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    OrdersCount = c.Orders.Count
                }).ToList();

            return new PagedList<CustomerDto> { HasNext = pagedCustomers.HasNext, TotalCount = pagedCustomers.TotalCount, Items = customerDtos };
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) 
                throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0) 
                throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) 
                throw new InvalidOperationException($"Customer Id {customerId} does not exist");

            //Business Rule: A customer can purchase only a single time per month
            if (await HasPurchasedThisMonth(customerId))
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            if (await IsFirstPurchaseOverLimit(customerId, purchaseValue))
                return false;

            //Business Rule: A customer can purchases only during business hours and working days
            if (IsOutsideBusinessHours())
                return false;

            return true;
        }

        private async Task<bool> HasPurchasedThisMonth(int customerId)
        {
            var baseDate = DateTimeProvider.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            return ordersInThisMonth > 0;
        }

        private async Task<bool> IsFirstPurchaseOverLimit(int customerId, decimal purchaseValue)
        {
            var haveBoughtBefore = await _ctx.Customers.AnyAsync(s => s.Id == customerId && s.Orders.Any());
            return !haveBoughtBefore && purchaseValue > 100;
        }

        private bool IsOutsideBusinessHours()
        {
            var hour = DateTimeProvider.UtcNow.Hour;
            var dayOfWeek = DateTimeProvider.UtcNow.DayOfWeek;
            return hour < 8 || hour > 18 || dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
        }
    }
}
