using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class RouteStationDtoValidator : AbstractValidator<RouteStationDto>
    {
        public RouteStationDtoValidator()
        {
            RuleFor(x => x.StationId)
                .GreaterThan(0).WithMessage("StationId must be a positive number");

            RuleFor(x => x.Order)
                .GreaterThan(0).WithMessage("Order must be a positive number");

            RuleFor(x => x.ArrivalOffsetMinutes)
                .GreaterThanOrEqualTo(0).WithMessage("ArrivalOffsetMinutes cannot be negative");

            RuleFor(x => x.StopDuration)
                .GreaterThanOrEqualTo(0).WithMessage("StopDuration cannot be negative");
        }
    }
}
