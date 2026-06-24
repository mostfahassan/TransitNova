using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;

namespace TransitNova.BusinessLayer.Features.Vehicles.Commands.CommandsValidators
{
    public sealed class DeleteVehicleCommandValidator : AbstractValidator<DeleteVehicleCommand>
    {
        public DeleteVehicleCommandValidator(IVehicleQueryRepository vehicleRepository)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Vehicle id is required.")
                .MustAsync(vehicleRepository.ExistsAsync)
                .WithMessage("Vehicle not found.");
        }
    }
}
