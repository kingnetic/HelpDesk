namespace HelpDesk.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendTicketReplyNotificationAsync(string toEmail, int ticketId, string reply);
        Task SendTicketAssignedNotificationAsync(string toEmail, int ticketId, string title);
        Task SendTicketAssignedNotificationAsync(string toEmail, int ticketId, string title, string assignedBy);
        Task SendTicketClosedNotificationAsync(string toEmail, int ticketId);
        Task SendTicketResolvedNotificationAsync(string toEmail, int ticketId);
    }
}
