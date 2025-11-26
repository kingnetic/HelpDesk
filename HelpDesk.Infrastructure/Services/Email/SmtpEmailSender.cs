using System;
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

        private async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail ?? _settings.Username, _settings.FromName ?? "HelpDesk System"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true // ✅ Cambiado a true para soportar HTML
            };

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        // --------------------------------------------------------------------
        // Confirmación de email
        // --------------------------------------------------------------------
        public async Task SendEmailConfirmationAsync(string toEmail, string fullName, string confirmationLink)
        {
            var subject = "Confirma tu cuenta en HelpDesk";
            var htmlBody = EmailTemplates.EmailConfirmation(fullName, confirmationLink);

            await SendAsync(toEmail, subject, htmlBody);
        }

        // --------------------------------------------------------------------
        // Notificación de cambio de contraseña
        // --------------------------------------------------------------------
        public async Task SendPasswordChangedNotificationAsync(string toEmail, string fullName)
        {
            var subject = "Tu contraseña ha sido cambiada";
            var htmlBody = EmailTemplates.PasswordChanged(fullName, DateTime.UtcNow);

            await SendAsync(toEmail, subject, htmlBody);
        }

        // --------------------------------------------------------------------
        // Notificación de respuesta
        // --------------------------------------------------------------------
        public async Task SendTicketReplyNotificationAsync(string toEmail, int ticketId, string reply)
        {
            var subject = $"Nueva respuesta en tu Ticket #{ticketId}";
            var htmlBody = EmailTemplates.TicketReply(ticketId, reply);

            await SendAsync(toEmail, subject, htmlBody);
        }

        // --------------------------------------------------------------------
        // Notificación de asignación (con assignedBy)
        // --------------------------------------------------------------------
        public async Task SendTicketAssignedNotificationAsync(string toEmail, int ticketId, string title, string assignedBy)
        {
            var subject = $"Ticket #{ticketId} asignado a ti";
            var htmlBody = EmailTemplates.TicketAssigned(ticketId, title, assignedBy);

            await SendAsync(toEmail, subject, htmlBody);
        }

        // --------------------------------------------------------------------
        // Notificación de asignación (sin assignedBy)
        // --------------------------------------------------------------------
        public async Task SendTicketAssignedNotificationAsync(string toEmail, int ticketId, string title)
        {
            var subject = $"Nuevo ticket asignado: #{ticketId}";
            var htmlBody = EmailTemplates.TicketAssigned(ticketId, title, "Sistema");

            await SendAsync(toEmail, subject, htmlBody);
        }

        // --------------------------------------------------------------------
        // Ticket cerrado
        // --------------------------------------------------------------------
        public async Task SendTicketClosedNotificationAsync(string toEmail, int ticketId)
        {
            var subject = $"Ticket #{ticketId} ha sido cerrado";
            var htmlBody = EmailTemplates.TicketClosed(ticketId);

            await SendAsync(toEmail, subject, htmlBody);
        }

        // --------------------------------------------------------------------
        // Ticket resuelto
        // --------------------------------------------------------------------
        public async Task SendTicketResolvedNotificationAsync(string toEmail, int ticketId)
        {
            var subject = $"Ticket #{ticketId} ha sido resuelto";
            var htmlBody = EmailTemplates.TicketResolved(ticketId);

            await SendAsync(toEmail, subject, htmlBody);
        }
    }
}
