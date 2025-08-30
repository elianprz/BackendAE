// EmailService.cs
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration; // Asegúrate de tener este 'using'
using System.IO;

namespace BackendAE.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string templatePath, Dictionary<string, string> replacements)
        {
            var email = new MimeMessage();

            // Asegúrate de que esta clave coincide con tu appsettings.json
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            if (string.IsNullOrEmpty(senderEmail))
            {
                throw new InvalidOperationException("La configuración del remitente (EmailSettings:SenderEmail) es nula o vacía.");
            }
            email.From.Add(MailboxAddress.Parse(senderEmail));

            // Si el nombre del remitente es opcional, también debes manejarlo
            var senderName = _configuration["EmailSettings:SenderName"];
            email.From.Add(new MailboxAddress(senderName, senderEmail));

            // Revisa si el correo del destinatario es nulo. Aunque tu DTO lo requiere,
            // esta validación extra es una buena práctica.
            if (string.IsNullOrEmpty(toEmail))
            {
                throw new ArgumentException("El correo del destinatario no puede ser nulo o vacío.", nameof(toEmail));
            }
            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            // Lee la plantilla y reemplaza los marcadores
            var body = File.ReadAllText(templatePath);
            foreach (var replacement in replacements)
            {
                body = body.Replace(replacement.Key, replacement.Value);
            }

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:Port"]);
            var smtpUser = _configuration["EmailSettings:SenderEmail"];
            var smtpPass = _configuration["EmailSettings:Password"];

            await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}