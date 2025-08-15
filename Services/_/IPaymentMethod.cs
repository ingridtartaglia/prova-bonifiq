namespace ProvaPub.Services
{
    public interface IPaymentMethod
    {
        Task Pay(decimal paymentValue, int customerId);
    }

    public class PixPaymentMethod : IPaymentMethod
    {
        public async Task Pay(decimal paymentValue, int customerId)
        {
        }
    }

    public class PaypalPaymentMethod : IPaymentMethod
    {
        public async Task Pay(decimal paymentValue, int customerId)
        {
        }
    }

    public class CreditCardPaymentMethod : IPaymentMethod
    {
        public async Task Pay(decimal paymentValue, int customerId)
        {
        }
    }
}
