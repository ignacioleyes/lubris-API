﻿namespace Lubris_API.Entities
{
    public class Sale: AuditableEntity
    {
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
