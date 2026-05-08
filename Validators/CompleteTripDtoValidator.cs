using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class CompleteTripDtoValidator : AbstractValidator<CompleteTripDto>
    {
        public CompleteTripDtoValidator()
        {
            RuleFor(x => x.ActualArrivalTime)
                .NotEmpty().WithMessage("Actual arrival time is required")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Actual arrival time cannot be in the future");
            ;
        }
    }
}
