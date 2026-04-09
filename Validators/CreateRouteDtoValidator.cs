using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class CreateRouteDtoValidator : AbstractValidator<CreateRouteDto>
    {
        public CreateRouteDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Route name is required")
                .MaximumLength(100).WithMessage("Route name cannot exceed 100 characters");

            RuleFor(x => x.Stations)
                .NotEmpty().WithMessage("Route must have at least one station");

            RuleForEach(x => x.Stations).SetValidator(new RouteStationDtoValidator());

            // order is unique for every station on route
            RuleFor(x => x.Stations)
                .Must(stations => stations.GroupBy(s => s.Order).All(g => g.Count() == 1))
                .WithMessage("Duplicate order values are not allowed");

            RuleFor(x => x.Stations)
                .Must(stations => stations.GroupBy(s => s.StationId).All(g => g.Count() == 1))
                .WithMessage("Duplicate stations are not allowed");

            // check if first stations has ArrivalOffsetMinutes 0
            RuleFor(x => x.Stations)
                .Must(stations => stations.OrderBy(s => s.Order).First().ArrivalOffsetMinutes == 0)
                .When(x => x.Stations != null && x.Stations.Count != 0)
                .WithMessage("The first station must have ArrivalOffsetMinutes of 0");

            // each station offset must be greater than the previous 
            RuleFor(x => x.Stations)
                .Must(stations =>
                {
                    var ordered = stations.OrderBy(s => s.Order).ToList();

                    for (int i = 1; i < ordered.Count; i++)
                    {
                        if (ordered[i].ArrivalOffsetMinutes <= ordered[i - 1].ArrivalOffsetMinutes)
                            return false;
                    }
                    return true;
                })
                .When(x => x.Stations != null && x.Stations.Count > 1)
                .WithMessage("Each station's ArrivalOffsetMinutes must be greater than the previous station's");
        }
    }
}
