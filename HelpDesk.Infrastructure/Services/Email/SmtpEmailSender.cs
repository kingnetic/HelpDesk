using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HelpDesk.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace HelpDesk.Infrastructure.Services.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailSender(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }

        private async Task SendAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.Username, "HelpDesk System"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        // --------------------------------------------------------------------
        // Reply notification
        // --------------------------------------------------------------------
        public async Task SendTicketReplyNotificationAsync(string toEmail, int ticketId, string reply)
        {
            var subject = $"Nueva respuesta en tu Ticket #{ticketId}";
            var body =
                $"Tu ticket #{ticketId} tiene una nueva respuesta:\n\n" +
                $"{reply}\n\n" +
                $"Accede al portal del HelpDesk para más detalles.";

            await SendAsync(toEmail, subject, body);
        }

        public async Task SendTicketAssignedNotificationAsync(string toEmail, int ticketId, string title, string assignedBy)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.Username, "HelpDesk System"),
                Subject = $"Asignación del Ticket #{ticketId}",
                Body = $@"
                        El ticket #{ticketId} ha sido asignado a usted.

                        Título: {title}
                        Asignado por: {assignedBy}

                        Por favor revise el HelpDesk para continuar.",
                IsBodyHtml = false
            };

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        // --------------------------------------------------------------------
        // Assignment notification
        // --------------------------------------------------------------------
        public async Task SendTicketAssignedNotificationAsync(string toEmail, int ticketId, string title)
        {
            var subject = $"Nuevo ticket asignado: #{ticketId}";
            var body =
                $"Se te ha asignado el ticket #{ticketId}:\n" +
                $"Título: {title}\n\n" +
                $"Por favor revisa la plataforma del HelpDesk.";

            await SendAsync(toEmail, subject, body);
        }

        // --------------------------------------------------------------------
        // Ticket closed
        // --------------------------------------------------------------------
        public async Task SendTicketClosedNotificationAsync(string toEmail, int ticketId)
        {
            var subject = $"Ticket #{ticketId} ha sido cerrado";
            var body =
                $"Tu ticket #{ticketId} ha sido cerrado.\n\n" +
                $"Gracias por usar HelpDesk.";

            await SendAsync(toEmail, subject, body);
        }

        // --------------------------------------------------------------------
        // Ticket resolved
        // --------------------------------------------------------------------
        public async Task SendTicketResolvedNotificationAsync(string toEmail, int ticketId)
        {
            var subject = $"Ticket #{ticketId} ha sido resuelto";
            var body =
                $"Tu ticket #{ticketId} ha sido marcado como RESUELTO.\n\n" +
                $"Si necesitas más ayuda, abre un ticket nuevo.";

            await SendAsync(toEmail, subject, body);
        }
    }
}
