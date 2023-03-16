namespace Lubris_API.Entities
{
    public class OilFilter: AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}
