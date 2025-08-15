﻿namespace ProvaPub.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public CustomerDto Customer { get; set; }
    }
}
