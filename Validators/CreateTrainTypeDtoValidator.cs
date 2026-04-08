using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class CreateTrainTypeDtoValidator : AbstractValidator<CreateTrainTypeDto>
    {
        public CreateTrainTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.MaxSpeed)
                .InclusiveBetween(1, 500).WithMessage("Max speed must be between 1 and 500 km/h");

            RuleFor(x => x.Capacity)
                .InclusiveBetween(1, 2000).WithMessage("Capacity must be between 1 and 2000");

            RuleFor(x => x.Manufacturer)
                .MaximumLength(100).WithMessage("Manufacturer cannot exceed 100 characters");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid train type");
        }
    }
}
