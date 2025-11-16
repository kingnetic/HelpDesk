using MediatR;
using HelpDesk.Application.DTOs.Tickets;

namespace HelpDesk.Application.Commands
{
    public record AssignTicketCommand(
        int TicketId,
        int EmployeeId,
        string? Ip,
        string? UserAgent
    ) : IRequest<AssignTicketResultDto>;
}
