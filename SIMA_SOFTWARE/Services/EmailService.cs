using Microsoft.Extensions.Options;
using SIMA_SOFTWARE.Configuration;
using System.Net;
using System.Net.Mail;

namespace SIMA_SOFTWARE.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task EnviarEmail(string destino, string asunto, string mensaje)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(_settings.Email);
            mail.To.Add(destino);
            mail.Subject = asunto;
            mail.Body = mensaje;
            mail.IsBodyHtml = true;

            var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Email, _settings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
