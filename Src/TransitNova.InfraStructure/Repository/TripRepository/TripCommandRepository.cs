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
        public async Task StartNewTripAsync(Trip trip, CancellationToken cancellationToken)
        {
            logger.LogTrace("Starting a new trip with ID: {TripId}", trip.Id);
            await context.Trips.AddAsync(trip, cancellationToken);
        }

        public async Task<Trip?> GetTripForCommandsAsync(Guid tripId, CancellationToken cancellationToken)
            => await context.Trips
                .Include(t => t.Shipments)
                .FirstOrDefaultAsync(t => t.Id == tripId, cancellationToken);

     
    }
}
