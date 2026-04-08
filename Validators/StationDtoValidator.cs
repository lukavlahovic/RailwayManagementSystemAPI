using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class StationDtoValidator : AbstractValidator<StationDto>
    {
        public StationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Station name is required")
                .MaximumLength(100).WithMessage("Station name cannot exceed 100 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

            RuleFor(x => x.NumberOfPlatforms)
                .InclusiveBetween(0, 20).WithMessage("Number of platforms must be between 0 and 20");
        }
    }
}
