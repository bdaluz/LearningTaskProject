using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System.Net;
using System.Net.Mail;


namespace Services.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        private readonly string _fromEmail = configuration["EmailSettings:email"]!;
        private readonly string _smtpPassword = configuration["EmailSettings:smtpPassword"]!;
        private readonly string _smtpClient = configuration["EmailSettings:smtpServer"]!;
        private readonly int _smtpPort = int.Parse(configuration["EmailSettings:smtpPort"]!);

        public async Task SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_fromEmail, "TaskProject");
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    mail.Body = body;

                    using (SmtpClient smtpClient = new SmtpClient(_smtpClient))
                    {
                        smtpClient.Port = _smtpPort;
                        smtpClient.Credentials = new NetworkCredential(_fromEmail, _smtpPassword);
                        smtpClient.EnableSsl = true;

                        smtpClient.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error to send email: " + ex.Message);
            }
        }
    }
}
