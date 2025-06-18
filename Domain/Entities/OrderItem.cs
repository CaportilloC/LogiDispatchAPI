﻿namespace Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        // Navigation
        public Order Order { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}
