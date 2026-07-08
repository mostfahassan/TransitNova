using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Features.Carriers
{
    internal static class CarrierProfileBuilder
    {
        public static CarrierProfileDto FromCarrier(Carrier carrier)
        {
            return new CarrierProfileDto
            {
                Id = carrier.Id,
                FullName = carrier.FullName,
                Email = carrier.Email,
                PhoneNumber = carrier.PhoneNumber,
                Address = carrier.Address,
                UserType = carrier.UserType,
                CityName = carrier.City?.Name ?? string.Empty,
                GovernmentName = carrier.City?.Government?.Name ?? string.Empty,
                Code = carrier.Code,
                ContractStartDate = carrier.ContractStartDate,
                ContractEndDate = carrier.ContractEndDate,
                Rating = carrier.AverageRating,
                LicenseNumber = carrier.LicenseNumber,
                Experience = carrier.YearsOfExperience,
                DefaultCostPerKg = carrier.DefaultCostPerKg,
                Status = carrier.Status,
                Vehicle = carrier.Vehicle is null
                    ? null
                    : new CarrierVehicleDto
                    {
                        Id = carrier.Vehicle.Id,
                        VehicleType = carrier.Vehicle.VehicleType,
                        PlateNumber = carrier.Vehicle.PlateNumber,
                        CapacityWeight = carrier.Vehicle.CapacityWeight,
                        CapacityVolume = carrier.Vehicle.CapacityVolume,
                        IsRefrigerated = carrier.Vehicle.IsRefrigerated,
                        IsActive = carrier.Vehicle.IsActive
                    }
            };
        }
    }
}