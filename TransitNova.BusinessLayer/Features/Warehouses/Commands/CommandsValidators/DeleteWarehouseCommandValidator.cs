using FluentValidation;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Warehouses.Commands.CommandsValidators
{
    public sealed class DeleteWarehouseCommandValidator : AbstractValidator<DeleteWarehouseCommand>
    {
        public DeleteWarehouseCommandValidator()
        {
            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage($"{ErrorCode.REQUIRED_FIELD}");
               
        }
    }
}
