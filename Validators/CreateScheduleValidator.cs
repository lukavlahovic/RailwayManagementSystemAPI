using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class CreateScheduleValidator : AbstractValidator<CreateScheduleDto>
    {
        public CreateScheduleValidator()
        {
            RuleFor(x => x.TrainId)
                .GreaterThan(0).WithMessage("TrainId must be a positive number");

            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("RouteId must be a positive number");

            RuleFor(x => x.DepartureTime)
                .NotEmpty().WithMessage("Departure time is required");

            RuleFor(x => x.ScheduleType)
                .IsInEnum().WithMessage("Not valid type of schedule");

            RuleFor(x => x.ValidFrom)
                .NotEmpty().WithMessage("ValidFrom is required");

            RuleFor(x => x.ValidTo)
                .Must((dto, validTo) => validTo > dto.ValidFrom)
                .When(x => x.ValidTo.HasValue)
                .WithMessage("ValidTo is either empty or it must be after ValidFrom");
        }
    }
}