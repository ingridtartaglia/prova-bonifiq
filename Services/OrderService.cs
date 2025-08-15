using Microsoft.EntityFrameworkCore;
using ProvaPub.DTOs;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
	public class OrderService
	{
        private readonly TestDbContext _ctx;

        public OrderService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<OrderDto> PayOrder(IPaymentMethod paymentMethod, decimal paymentValue, int customerId)
		{
            await paymentMethod.Pay(paymentValue, customerId);

            var order = new Order() { Value = paymentValue, CustomerId = customerId, OrderDate = DateTime.UtcNow };

            await _ctx.Orders.AddAsync(order);
            await _ctx.SaveChangesAsync();

            var savedOrder = await _ctx.Orders
                                    .Include(o => o.Customer)
                                    .ThenInclude(c => c.Orders)
                                    .FirstOrDefaultAsync(o => o.Id == order.Id);

            return new OrderDto
            {
                Id = savedOrder.Id,
                Value = savedOrder.Value,
                CustomerId = savedOrder.CustomerId,
                OrderDate = savedOrder.OrderDate,
                Customer = new CustomerDto
                {
                    Id = savedOrder.Customer.Id,
                    Name = savedOrder.Customer.Name,
                    OrdersCount = savedOrder.Customer.Orders?.Count ?? 0
                }
            };
        }
	}
}
