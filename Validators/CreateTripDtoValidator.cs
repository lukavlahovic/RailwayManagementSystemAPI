using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class CreateTripDtoValidator : AbstractValidator<CreateTripDto>
    {
        public CreateTripDtoValidator()
        {
            RuleFor(x => x.TrainId)
                .GreaterThan(0).WithMessage("TrainId must be a positive number");

            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("RouteId must be a positive number");

            RuleFor(x => x.DepartureTime)
                .NotEmpty().WithMessage("Departure time is required")
                .GreaterThan(DateTime.Now).WithMessage("Departure time must be in the future");

            RuleFor(x => x.ArrivalTime)
                .NotEmpty().WithMessage("Arrival time is required")
                .GreaterThan(x => x.DepartureTime).WithMessage("Arrival time must be after departure time");
        }
    }
}
