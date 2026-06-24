using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Admin.Queries
{
    public sealed record GetAdminDashboardQuery : IQuery<Result<AdminDashboardDto>>, ICachable
    {
        public string CacheKey => CacheKeys.AdminDashboard();
    }
}
