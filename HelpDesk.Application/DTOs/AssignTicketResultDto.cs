namespace HelpDesk.Application.DTOs.Tickets
{
    public record AssignTicketResultDto(int TicketId, int EmployeeId, string EmployeeEmail, DateTime AssignedAt, string Status);
}
