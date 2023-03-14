namespace Lubris_API.Entities
{
    public class Product: AuditableEntity
    {
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public float CostPrice { get; set; }
        public int ProfitMargin { get; set; }
        public float SellingPrice => (CostPrice * ProfitMargin / 100) + CostPrice;
        public List<string> ImagesUrls { get; set; }
        public ProductTypeEnum ProductType { get; set; }
        public BrandEnum Brand { get; set; }
        public List<Sale> Sales { get; set; }

    }

    public enum ProductTypeEnum
    {
        BulkOils,
        PackagedOils,
        CarFilters,
        PackagedCoolant
    }

    public enum BrandEnum
    {
        YPF,
        Shell,
        Petronas,
        Total,
        Wega,
        Valvoline,
        MannFilter,
        Mahle,
        Fram

    }
}
