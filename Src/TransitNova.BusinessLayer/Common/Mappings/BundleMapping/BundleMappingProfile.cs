using AutoMapper;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.Domain.Entities.MainEntities;
public class BundleMappingProfile:Profile
 {
        public BundleMappingProfile()
        {
            // Bundle → Retrieve
            CreateMap<Bundle, RetrieveBundleDto>();
        }


 }

