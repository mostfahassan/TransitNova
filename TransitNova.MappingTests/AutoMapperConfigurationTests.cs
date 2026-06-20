using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.DTOs.UserProfile.Admin;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.ServiceRegistration;
using Xunit;

namespace TransitNova.MappingTests
{
    public class AutoMapperConfigurationTests
    {
        [Fact]
        public void All_registered_profiles_are_valid()
        {
            var configuration = CreateConfiguration();

            configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Repository_project_to_maps_translate_to_sql()
        {
            var configuration = CreateConfiguration();
            using var context = CreateDbContext();

            _ = context.Shipments.ProjectTo<RetrieveShipmentDto>(configuration).ToQueryString();
            _ = context.ShipmentStatuses.ProjectTo<RetrieveShipmentStatusDto>(configuration).ToQueryString();
            _ = context.Payments.ProjectTo<PaymentSummaryDto>(configuration).ToQueryString();
            _ = context.Vehicles.ProjectTo<VehicleDto>(configuration).ToQueryString();
            _ = context.Carriers.ProjectTo<CarrierProfileDto>(configuration).ToQueryString();
            _ = context.CarrierCompanies.ProjectTo<RetrieveCarrierCompany>(configuration).ToQueryString();
            _ = context.UserProfiles.ProjectTo<UserProfileDto>(configuration).ToQueryString();
            _ = context.UserProfiles.ProjectTo<UserSummaryDto>(configuration).ToQueryString();
            _ = context.ReceiverProfiles.ProjectTo<UserSummaryDto>(configuration).ToQueryString();
            _ = context.OperationManagerProfiles.ProjectTo<UserSummaryDto>(configuration).ToQueryString();
            _ = context.Admins.ProjectTo<AdminProfileDto>(configuration).ToQueryString();
            _ = context.Admins.ProjectTo<UserSummaryDto>(configuration).ToQueryString();
            _ = context.Countries.ProjectTo<CountryDto>(configuration).ToQueryString();
            _ = context.Cities.ProjectTo<CityDto>(configuration).ToQueryString();
            _ = context.Zones.ProjectTo<ZoneDto>(configuration).ToQueryString();
            _ = context.Bundles.ProjectTo<RetrieveBundleDto>(configuration).ToQueryString();
        }

        private static MapperConfiguration CreateConfiguration()
            => new(cfg =>
            {
                cfg.AddMaps(typeof(BusinessLayer.DependencyInjection).Assembly);
                cfg.AddMaps(typeof(DependencyInjection).Assembly);
            }, NullLoggerFactory.Instance);

        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TransitNovaMappingTests;Trusted_Connection=True;")
                .Options;

            return new AppDbContext(options);
        }
    }
}
