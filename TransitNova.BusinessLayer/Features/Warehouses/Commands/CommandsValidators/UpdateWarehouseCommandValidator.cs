using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Warehouses.Commands.CommandsValidators
{
    public sealed class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
    {
        public UpdateWarehouseCommandValidator(
            IValidator<UpdateWarehouseDto> dtoValidator,
            IWarehouseRulesRepository warehouseRepository)
        {
            RuleFor(x => x.AdminId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage("Warehouse id is required.")
                .MustAsync(warehouseRepository.ExistsAsync)
                .WithMessage("Warehouse not found.");

            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x)
                .MustAsync(async (command, ct) =>
                    !await warehouseRepository.ExistsByNameAsync(command.Dto.Name, command.WarehouseId, ct))
                .WithMessage("Warehouse name already exists.")
                .When(x => x.Dto is not null);
        }
    }
}
