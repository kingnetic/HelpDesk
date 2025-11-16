using FluentValidation;
using HelpDesk.Application.Commands;

namespace HelpDesk.Application.Validators
{
    public class AssignTicketCommandValidator : AbstractValidator<AssignTicketCommand>
    {
        public AssignTicketCommandValidator()
        {
            RuleFor(x => x.TicketId)
                .GreaterThan(0)
                .WithMessage("TicketId must be a positive number.");

            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("EmployeeId must be a valid user id.");

            RuleFor(x => x.Ip)
                .NotEmpty().WithMessage("IP address is required.")
                .MaximumLength(50).WithMessage("IP address is too long.");

            RuleFor(x => x.UserAgent)
                .NotEmpty().WithMessage("User-Agent is required.")
                .MaximumLength(500).WithMessage("User-Agent header is too long.");
        }
    }
}
