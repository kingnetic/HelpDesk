using FluentValidation;
using HelpDesk.Application.Commands;

namespace HelpDesk.Application.Validators
{
    public class ReplyTicketCommandValidator : AbstractValidator<ReplyTicketCommand>
    {
        public ReplyTicketCommandValidator()
        {
            RuleFor(x => x.TicketId)
                .GreaterThan(0)
                .WithMessage("El TicketId debe ser mayor a 0.");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("El UserId debe ser mayor a 0.");

            RuleFor(x => x.Comment)
                .NotEmpty()
                .WithMessage("El comentario no puede estar vacío.")
                .MinimumLength(3)
                .WithMessage("El comentario debe tener al menos 3 caracteres.");
        }
    }
}
