using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging; // ⭐ Nuevo: Para registrar errores

namespace BackendAE.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger; // ⭐ Nuevo: Para logging

        // ⭐ Constructor modificado para inyectar ILogger
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string templatePath, Dictionary<string, string> replacements)
        {
            try
            {
                var email = new MimeMessage();

                // ... (Validaciones y configuración de remitente/destinatario como ya lo tenías) ...
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];

                email.From.Add(new MailboxAddress(senderName, senderEmail));
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
            catch (Exception ex)
            {
                // ⭐ Manejo de errores: Registrar el fallo del correo y NO relanzar
                _logger.LogError(ex, "Error al enviar correo a {ToEmail} con asunto {Subject}. La solicitud HTTP principal no será bloqueada.", toEmail, subject);
                // No lanzamos la excepción para que el patrón Fire-and-Forget funcione sin matar la solicitud HTTP.
            }
        }
    }
}