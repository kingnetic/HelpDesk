using FluentValidation;
using HelpDesk.Application.Commands;

namespace HelpDesk.Application.Validators
{
    public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("El título es requerido.")
                .MaximumLength(250)
                .WithMessage("El título no puede exceder los 250 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("La descripción es requerida.");

            RuleFor(x => x.CreatedById)
                .GreaterThan(0)
                .WithMessage("El ID del creador debe ser válido.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("La categoría es requerida.");

            RuleFor(x => x.PriorityId)
                .GreaterThan(0)
                .WithMessage("La prioridad es requerida.");
        }
    }
}
