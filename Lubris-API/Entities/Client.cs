namespace Lubris_API.Entities
{
    public class Client : AuditableEntity
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public int CellPhone { get; set; }
        public List<Sale> Sales { get; set; }
    }
}
