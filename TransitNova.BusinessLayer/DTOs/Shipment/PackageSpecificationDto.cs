using TransitNova.Domain.Entities.Common;

namespace TransitNova.BusinessLayer.DTOs.Shipment
{
    public class PackageSpecificationDto
    {
        public decimal Weight { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }

        public PackageSpecification ToDomain()
            => new(Weight, Width, Height, Length);
    }

}
