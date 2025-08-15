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

        public async Task<Order> PayOrder(IPaymentMethod paymentMethod, decimal paymentValue, int customerId)
		{
            await paymentMethod.Pay(paymentValue, customerId);

            var order = new Order() { Value = paymentValue, CustomerId = customerId };

            await _ctx.Orders.AddAsync(order);
            await _ctx.SaveChangesAsync();

            return order;
        }
	}
}
