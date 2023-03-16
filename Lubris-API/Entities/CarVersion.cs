namespace Lubris_API.Entities
{
    public class CarVersion
    {
        public string Name { get; set; }
        public int CarBrandId { get; set; }
        public CarBrand CarBrand { get; set; }
        public int CarModelId { get; set; }
        public CarModel CarModel { get; set; }
    }
}
