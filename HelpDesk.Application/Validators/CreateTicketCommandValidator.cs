using FluentValidation;
using HelpDesk.Application.Commands;

namespace HelpDesk.Application.Validators
{
    public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.CreatedById).GreaterThan(0);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.PriorityId).GreaterThan(0);
        }
    }
}
