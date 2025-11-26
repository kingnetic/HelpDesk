namespace HelpDesk.Infrastructure.Services.Email
{
    public static class EmailTemplates
    {
        private static string GetBaseTemplate(string title, string content)
        {
            return $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #ffffff;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }}
        .content {{
            padding: 40px 30px;
            color: #333333;
            line-height: 1.6;
        }}
        .content h2 {{
            color: #667eea;
            font-size: 22px;
            margin-top: 0;
        }}
        .content p {{
            margin: 15px 0;
            font-size: 16px;
        }}
        .button {{
            display: inline-block;
            padding: 14px 30px;
            margin: 25px 0;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #ffffff !important;
            text-decoration: none;
            border-radius: 5px;
            font-weight: 600;
            font-size: 16px;
            transition: transform 0.2s;
        }}
        .button:hover {{
            transform: translateY(-2px);
        }}
        .info-box {{
            background-color: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 15px 20px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .warning-box {{
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px 20px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px 30px;
            text-align: center;
            color: #6c757d;
            font-size: 14px;
        }}
        .footer p {{
            margin: 5px 0;
        }}
        .divider {{
            height: 1px;
            background-color: #e9ecef;
            margin: 25px 0;
        }}
        .ticket-id {{
            background-color: #667eea;
            color: #ffffff;
            padding: 2px 8px;
            border-radius: 3px;
            font-weight: 600;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <h1>🎫 HelpDesk</h1>
        </div>
        <div class=""content"">
            {content}
        </div>
        <div class=""footer"">
            <p><strong>HelpDesk System</strong></p>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p>&copy; 2025 HelpDesk. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string EmailConfirmation(string fullName, string confirmationLink)
        {
            var content = $@"
                <h2>¡Bienvenido a HelpDesk! 👋</h2>
                <p>Hola <strong>{fullName}</strong>,</p>
                <p>Gracias por registrarte en nuestro sistema de HelpDesk. Estamos emocionados de tenerte con nosotros.</p>
                
                <div class=""info-box"">
                    <p style=""margin: 0;""><strong>📧 Confirma tu dirección de correo electrónico</strong></p>
                    <p style=""margin: 5px 0 0 0;"">Para activar tu cuenta y comenzar a usar HelpDesk, necesitamos verificar tu correo electrónico.</p>
                </div>

                <center>
                    <a href=""{confirmationLink}"" class=""button"">✓ Confirmar mi cuenta</a>
                </center>

                <p style=""font-size: 14px; color: #6c757d;"">O copia y pega este enlace en tu navegador:</p>
                <p style=""font-size: 13px; color: #6c757d; word-break: break-all;"">{confirmationLink}</p>

                <div class=""divider""></div>

                <div class=""warning-box"">
                    <p style=""margin: 0; font-size: 14px;""><strong>⚠️ ¿No creaste esta cuenta?</strong></p>
                    <p style=""margin: 5px 0 0 0; font-size: 14px;"">Si no solicitaste esta cuenta, puedes ignorar este mensaje de forma segura.</p>
                </div>

                <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                <p>Saludos cordiales,<br><strong>El equipo de HelpDesk</strong></p>
            ";

            return GetBaseTemplate("Confirma tu cuenta - HelpDesk", content);
        }

        public static string PasswordChanged(string fullName, DateTime changeDate)
        {
            var content = $@"
                <h2>Contraseña Cambiada Exitosamente 🔐</h2>
                <p>Hola <strong>{fullName}</strong>,</p>
                <p>Te confirmamos que tu contraseña ha sido cambiada exitosamente en HelpDesk.</p>

                <div class=""info-box"">
                    <p style=""margin: 0;""><strong>📅 Detalles del cambio:</strong></p>
                    <p style=""margin: 5px 0 0 0;"">
                        <strong>Fecha y hora:</strong> {changeDate:dd/MM/yyyy HH:mm:ss} UTC<br>
                    </p>
                </div>

                <div class=""divider""></div>

                <div class=""warning-box"">
                    <p style=""margin: 0; font-size: 14px;""><strong>⚠️ ¿No realizaste este cambio?</strong></p>
                    <p style=""margin: 5px 0 0 0; font-size: 14px;"">Si no fuiste tú quien cambió la contraseña, por favor contacta al administrador <strong>inmediatamente</strong> para asegurar tu cuenta.</p>
                </div>

                <p>Por seguridad, te recomendamos:</p>
                <ul>
                    <li>Usar contraseñas únicas y seguras</li>
                    <li>No compartir tu contraseña con nadie</li>
                    <li>Cambiar tu contraseña regularmente</li>
                </ul>

                <p>Saludos cordiales,<br><strong>El equipo de HelpDesk</strong></p>
            ";

            return GetBaseTemplate("Contraseña Cambiada - HelpDesk", content);
        }

        public static string TicketReply(int ticketId, string reply)
        {
            var content = $@"
                <h2>Nueva Respuesta en tu Ticket 💬</h2>
                <p>Tu ticket <span class=""ticket-id"">#{ticketId}</span> tiene una nueva respuesta.</p>

                <div class=""info-box"">
                    <p style=""margin: 0;""><strong>📝 Respuesta:</strong></p>
                    <p style=""margin: 10px 0 0 0;"">{reply}</p>
                </div>

                <center>
                    <a href=""#"" class=""button"">Ver Ticket Completo</a>
                </center>

                <p>Accede al portal de HelpDesk para ver todos los detalles y continuar la conversación.</p>

                <p>Saludos cordiales,<br><strong>El equipo de HelpDesk</strong></p>
            ";

            return GetBaseTemplate($"Nueva Respuesta - Ticket #{ticketId}", content);
        }

        public static string TicketAssigned(int ticketId, string title, string assignedBy)
        {
            var content = $@"
                <h2>Ticket Asignado 📋</h2>
                <p>Se te ha asignado un nuevo ticket en HelpDesk.</p>

                <div class=""info-box"">
                    <p style=""margin: 0;""><strong>Ticket:</strong> <span class=""ticket-id"">#{ticketId}</span></p>
                    <p style=""margin: 5px 0 0 0;""><strong>Título:</strong> {title}</p>
                    <p style=""margin: 5px 0 0 0;""><strong>Asignado por:</strong> {assignedBy}</p>
                </div>

                <center>
                    <a href=""#"" class=""button"">Ver Ticket</a>
                </center>

                <p>Por favor revisa el ticket y proporciona una respuesta lo antes posible.</p>

                <p>Saludos cordiales,<br><strong>El equipo de HelpDesk</strong></p>
            ";

            return GetBaseTemplate($"Ticket Asignado - #{ticketId}", content);
        }

        public static string TicketClosed(int ticketId)
        {
            var content = $@"
                <h2>Ticket Cerrado ✓</h2>
                <p>Tu ticket <span class=""ticket-id"">#{ticketId}</span> ha sido cerrado.</p>

                <div class=""info-box"">
                    <p style=""margin: 0;"">El ticket ha sido marcado como cerrado. Si necesitas más ayuda, puedes abrir un nuevo ticket en cualquier momento.</p>
                </div>

                <center>
                    <a href=""#"" class=""button"">Crear Nuevo Ticket</a>
                </center>

                <p>Gracias por usar HelpDesk. Esperamos haber resuelto tu problema satisfactoriamente.</p>

                <p>Saludos cordiales,<br><strong>El equipo de HelpDesk</strong></p>
            ";

            return GetBaseTemplate($"Ticket Cerrado - #{ticketId}", content);
        }

        public static string TicketResolved(int ticketId)
        {
            var content = $@"
                <h2>Ticket Resuelto ✓</h2>
                <p>Tu ticket <span class=""ticket-id"">#{ticketId}</span> ha sido marcado como <strong>RESUELTO</strong>.</p>

                <div class=""info-box"">
                    <p style=""margin: 0;"">Nuestro equipo ha trabajado en tu solicitud y la ha marcado como resuelta. Si el problema persiste o tienes alguna pregunta adicional, no dudes en contactarnos.</p>
                </div>

                <center>
                    <a href=""#"" class=""button"">Ver Detalles</a>
                </center>

                <p>Si necesitas más ayuda, puedes:</p>
                <ul>
                    <li>Reabrir este ticket si el problema no está completamente resuelto</li>
                    <li>Crear un nuevo ticket para un problema diferente</li>
                </ul>

                <p>Saludos cordiales,<br><strong>El equipo de HelpDesk</strong></p>
            ";

            return GetBaseTemplate($"Ticket Resuelto - #{ticketId}", content);
        }
    }
}
