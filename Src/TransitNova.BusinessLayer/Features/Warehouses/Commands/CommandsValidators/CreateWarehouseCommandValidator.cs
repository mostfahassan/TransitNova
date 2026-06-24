using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.Warehouses.Commands.CommandsValidators
{
    public sealed class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
    {
        public CreateWarehouseCommandValidator(
            IValidator<CreateWarehouseDto> dtoValidator,
            IWarehouseRulesRepository warehouseRepository)
        {
            RuleFor(x => x.AdminId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.Dto.Name)
                .MustAsync(async (name, ct) =>
                    !await warehouseRepository.ExistsByNameAsync(name, null, ct))
                .WithMessage("Warehouse name already exists.")
                .When(x => x.Dto is not null);
        }
    }
}
