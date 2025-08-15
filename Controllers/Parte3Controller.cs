﻿using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Services;

namespace ProvaPub.Controllers
{

    /// <summary>
    /// Esse teste simula um pagamento de uma compra.
    /// O método PayOrder aceita diversas formas de pagamento. Dentro desse método é feita uma estrutura de diversos "if" para cada um deles.
    /// Sabemos, no entanto, que esse formato não é adequado, em especial para futuras inclusões de formas de pagamento.
    /// Como você reestruturaria o método PayOrder para que ele ficasse mais aderente com as boas práticas de arquitetura de sistemas?
    /// 
    /// Outra parte importante é em relação à data (OrderDate) do objeto Order. Ela deve ser salva no banco como UTC mas deve retornar para o cliente no fuso horário do Brasil. 
    /// Demonstre como você faria isso.
    /// </summary>
    [ApiController]
	[Route("[controller]")]
	public class Parte3Controller : ControllerBase
	{
        private readonly OrderService _orderService;

        public Parte3Controller(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("orders")]
		public async Task<Order> PlaceOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
            IPaymentMethod paymentStrategy;

            switch (paymentMethod.ToLower())
            {
                case "pix":
                    paymentStrategy = new PixPaymentMethod();
                    break;
                case "creditcard":
                    paymentStrategy = new CreditCardPaymentMethod();
                    break;
                case "paypal":
                    paymentStrategy = new PaypalPaymentMethod();
                    break;
                default:
                    throw new NotSupportedException("Not supported payment method");
            }

            var order = await _orderService.PayOrder(paymentStrategy, paymentValue, customerId);

            var brazilianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            order.OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, brazilianTimeZone);

            return order;
        }
    }
}
