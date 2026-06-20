
using System.Linq.Expressions;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository
{
    public interface ICarrierQueryRepository
    {
        Task<TRetrieve?> GetCarrierDetailsAsync<TRetrieve>(Guid carrierId, CancellationToken ct = default);
        Task<Carrier?> GetCarrierAsync(Expression<Func<Carrier, bool>> predicate, CancellationToken cancellationToken =default);
        Task<Guid> GetCarrierIdByUserIdAsync(Guid appUserId, CancellationToken ct = default);
        Task<string?> GetCarrierNameAsync(Guid appUserId, CancellationToken ct = default);
        Task<CarrierStatus?> GetStatusAsync(Guid carrierId, CancellationToken ct = default);
        Task<PagedResult<CarrierSummaryDetailsDto>> GetCarriersServingCityAsync(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<IEnumerable<CarrierProfileDto>> GetCarriersInStatusAsync(CarrierStatus status, CancellationToken ct = default);
        Task<PagedResult<TRetrieve>> FilterByCriteriaAsync<TRetrieve>(FilterCarrierDto filterCriteria, bool tracked = false, CancellationToken ct = default);
        
    }
}
