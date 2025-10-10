using BackendAE.DTOs;
using BackendAE.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SoporteController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;

    public SoporteController(EmailService emailService, IConfiguration configuration)
    {
        _emailService = emailService;
        _configuration = configuration;
    }

    [HttpPost("enviar-mensaje")]
    public async Task<ActionResult> EnviarMensajeDeSoporte([FromBody] SoporteDTO dto)
    {
        var destinatario = _configuration["EmailSettings:SenderEmail"];
        var asunto = $"Mensaje de soporte de {dto.PrimerNombre} {dto.PrimerApellido}: {dto.Asunto}";

        // La ruta al nuevo template HTML
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Soporte.html");

        // El diccionario de reemplazos con los datos del formulario
        var replacements = new Dictionary<string, string>
        {
            { "@PrimerNombre", dto.PrimerNombre },
            { "@PrimerApellido", dto.PrimerApellido },
            { "@Correo", dto.Correo },
            { "@Asunto", dto.Asunto },
            { "@Mensaje", dto.Mensaje }
        };

        try
        {
            //await _emailService.SendEmailAsync(destinatario, asunto, templatePath, replacements);
            _emailService.SendEmailAsync(destinatario, asunto, templatePath, replacements);
            return Ok("Mensaje de soporte enviado correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar el correo de soporte: {ex.Message}");
            return StatusCode(500, "Ocurrió un error al enviar el mensaje. Por favor, inténtalo de nuevo más tarde.");
        }
    }
}