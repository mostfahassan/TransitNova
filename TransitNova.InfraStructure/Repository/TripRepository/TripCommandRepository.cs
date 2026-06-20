using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.TripRepository
{
    internal class TripCommandRepository(ILogger<TripCommandRepository> logger, AppDbContext context) : ITripCommandRepository
    {
        public async Task StartNewTrip(Trip trip, CancellationToken cancellationToken)
        {
            logger.LogTrace("Starting a new trip with ID: {TripId}", trip.Id);
            await context.Trips.AddAsync(trip, cancellationToken);
        }
    }
}
