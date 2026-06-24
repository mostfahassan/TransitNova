using AutoMapper;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Repository.Generic;
namespace TransitNova.InfraStructure.Repository.WarehouseRepo
{
    internal class WarehouseCommandRepository(AppDbContext context, IMapper mapper) : GenericRepository<Warehouse, Guid>(context, mapper.ConfigurationProvider), IWarehouseCommandsRepository
    {


    }
}
