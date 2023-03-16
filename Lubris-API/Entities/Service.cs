namespace Lubris_API.Entities
{
    public class Service: AuditableEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public float TotalPrice { get; set; }
    }
}
