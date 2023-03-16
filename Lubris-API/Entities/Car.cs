namespace Lubris_API.Entities
{
    public class Car: AuditableEntity
    {
        public string Name { get; set; }
        public int BrandId { get; set; }
        public CarBrand Brand { get; set; }
        public int ModelId { get; set; }
        public CarModel Model { get; set; }
        public int VersionId { get; set; }
        public CarVersion Version { get; set; }

    }
}
