using System;
using System.Threading;
using System.Threading.Tasks;
using HelpDesk.Application.Commands;
using HelpDesk.Application.DTOs.Tickets;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using MediatR;

namespace HelpDesk.Application.Handlers
{
    public class AssignTicketHandler : IRequestHandler<AssignTicketCommand, AssignTicketResultDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;
        private readonly IEmailSender _email;
        private readonly IAuditService _audit;

        public AssignTicketHandler(IUnitOfWork uow, IUserService userService, IEmailSender email, IAuditService audit)
        {
            _uow = uow;
            _userService = userService;
            _email = email;
            _audit = audit;
        }

        public async Task<AssignTicketResultDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            // Validar que el usuario/tecnico exista
            if (!await _userService.ExistsAsync(request.EmployeeId))
                throw new Exception("Technician does not exist.");

            var repo = _uow.Repository<Ticket>();

            var ticket = await repo.GetByIdAsync(request.TicketId)
                ?? throw new Exception("Ticket not found.");

            // Asignar el ticket usando el Rich Domain Model
            const int AssignedStatusId = 2; // "Assigned"
            ticket.AssignTo(request.EmployeeId, AssignedStatusId);

            repo.Update(ticket);
            await _uow.SaveChangesAsync(cancellationToken);

            var techEmail = await _userService.GetEmailByUserIdAsync(request.EmployeeId);

            // Enviar correo
            await _email.SendTicketAssignedNotificationAsync(techEmail, ticket.Id, ticket.Title, "SYSTEM");

            await _audit.LogAsync(ticketId: ticket.Id, userId: request.EmployeeId, action: "Ticket Assigned", detail: $"Assigned to user {request.EmployeeId}.", ip: request.Ip, userAgent: request.UserAgent);

            return new AssignTicketResultDto(TicketId: ticket.Id, EmployeeId: request.EmployeeId, EmployeeEmail: techEmail, AssignedAt: DateTime.UtcNow, Status: "Assigned");
        }
    }
}
