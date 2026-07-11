using AutoMapper;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.BusinessLayer.Common.Mappings.AddressMapping;

public sealed class AddressMappingProfile : Profile
{
    public AddressMappingProfile()
    {
        CreateMap<Address, AddressDto>();
        CreateMap<AddressDto, Address>()
            .ConstructUsing(source => source.ToDomain());
    }
}
