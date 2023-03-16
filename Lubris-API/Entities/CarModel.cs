namespace Lubris_API.Entities
{
    public class CarModel: AuditableEntity
    {
        public string Name  { get; set; }
        public int CarBrandId { get; set; }
        public CarBrand CarBrand { get; set; }
    }
}
