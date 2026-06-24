using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.BusinessLayer.Common.CommonData
{
    public interface IBaseLocationRepository<TEntity, TDto, TFilterDto> 
     where TEntity : BaseEntity<int>
     where TDto : IBaseLocationDto
    {
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken ct, int? id = null);
        Task<(List<TDto> Items, int TotalCount)> FilterAsync(TFilterDto filter, CancellationToken ct);
        Task<bool> NameExistsAsync(string name, CancellationToken ct ,int? id = null);
        Task<bool> NameExistsForAnotherAsync(int id,  string name, CancellationToken ct, int? SecondId = null);
    }
}

