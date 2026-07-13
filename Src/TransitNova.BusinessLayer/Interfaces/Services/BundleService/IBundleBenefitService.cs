using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.Shipment;

namespace TransitNova.BusinessLayer.Interfaces.Services.BundleService;

public interface IBundleBenefitService
{
    Task<BundleBenefitResultDto> CalculateShipmentBenefitAsync(Guid userProfileId, decimal originalShippingCost, PackageSpecificationDto packageSpecification, CancellationToken cancellationToken);

}
