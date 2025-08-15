using NUnit.Framework;
using ProvaPub.Services;
using ProvaPub.Repository;
using ProvaPub.Models;
using Microsoft.EntityFrameworkCore;

namespace ProvaPub.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private TestDbContext _ctx;
        private CustomerService _customerService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _ctx = new TestDbContext(options);
            _customerService = new CustomerService(_ctx);
        }

        [TearDown]
        public void TearDown()
        {
            _ctx.Database.EnsureDeleted();
            _ctx.Dispose();
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void CanPurchase_InvalidCustomerId_ThrowsException(int customerId)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _customerService.CanPurchase(customerId, 100));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task CanPurchase_InvalidPurchaseValue_ThrowsException(decimal purchaseValue)
        {
            _ctx.Customers.Add(new Customer { Id = 1, Name = "Valid Customer" });
            await _ctx.SaveChangesAsync();

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _customerService.CanPurchase(1, purchaseValue));
        }

        [Test]
        public void CanPurchase_NonExistentCustomer_ThrowsException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _customerService.CanPurchase(999, 100));
        }

        [Test]
        public async Task CanPurchase_AlreadyPurchasedThisMonth_ReturnsFalse()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "Customer" };
            var order = new Order { CustomerId = customerId, OrderDate = DateTime.UtcNow, Value = 50 };

            _ctx.Customers.Add(customer);
            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();

            // Act
            var result = await _customerService.CanPurchase(customerId, 100);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CanPurchase_FirstPurchaseOverLimit_ReturnsFalse()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "Customer" };
            _ctx.Customers.Add(customer);
            await _ctx.SaveChangesAsync();

            // Act
            var result = await _customerService.CanPurchase(customerId, 101);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CanPurchase_FirstPurchaseUnderLimit_ReturnsTrue()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "Customer" };
            _ctx.Customers.Add(customer);
            await _ctx.SaveChangesAsync();

            // Act
            var result = await _customerService.CanPurchase(customerId, 99);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CanPurchase_AlreadyPurchasedThisMonthInDifferentYear_ReturnsTrue()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "Customer" };
            var order = new Order { CustomerId = customerId, OrderDate = DateTime.UtcNow.AddYears(-1), Value = 50 };

            _ctx.Customers.Add(customer);
            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();

            // Act
            var result = await _customerService.CanPurchase(customerId, 100);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}