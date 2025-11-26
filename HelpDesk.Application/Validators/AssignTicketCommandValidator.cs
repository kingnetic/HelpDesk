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
                .WithMessage("El ID del ticket debe ser un número positivo.");

            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("El ID del empleado debe ser válido.");

            RuleFor(x => x.Ip)
                .NotEmpty().WithMessage("La dirección IP es requerida.")
                .MaximumLength(50).WithMessage("La dirección IP es demasiado larga.");

            RuleFor(x => x.UserAgent)
                .NotEmpty().WithMessage("El User-Agent es requerido.")
                .MaximumLength(500).WithMessage("El encabezado User-Agent es demasiado largo.");
        }
    }
}
