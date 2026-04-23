using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class CreateDelayDtoValidator : AbstractValidator<CreateDelayDto>
    {
        public CreateDelayDtoValidator()
        {
            RuleFor(x => x.TripId)
                .GreaterThan(0).WithMessage("TripId must be a positive number");

            RuleFor(x => x.StationId)
                .GreaterThan(0).WithMessage("StationId must be a positive number");

            RuleFor(x => x.DelayMinutes)
                .GreaterThan(0).WithMessage("Delay must be at least 1 minute");

            RuleFor(x => x.TypeOfDelay)
                .IsInEnum().WithMessage("Invalid delay type");

            RuleFor(x => x.Note)
                .MaximumLength(250).WithMessage("Note cannot exceed 250 characters");
        }
    }
}
