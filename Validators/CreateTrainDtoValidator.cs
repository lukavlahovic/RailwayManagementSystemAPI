using FluentValidation;
using RailwayManagementSystemAPI.Dtos;

namespace RailwayManagementSystemAPI.Validators
{
    public class CreateTrainDtoValidator : AbstractValidator<CreateTrainDto>
    {
        public CreateTrainDtoValidator()
        {
            RuleFor(x => x.TrainTypeId)
                .GreaterThan(0).WithMessage("TrainTypeId must be a positive number");

            RuleFor(x => x.SerialNumber)
                .NotEmpty()
                .MaximumLength(50).WithMessage("Serial number cannot exceed 50 characters");
        }
    }
}
