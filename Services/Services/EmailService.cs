using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Services.DTOs.User;
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
        private readonly string _webAppUrl = configuration["ClientSettings:WebAppUrl"]!;


        private async Task SendEmail(string toEmail, string subject, string body)
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

        public async Task SendPasswordResetEmail(string toEmail, string token)
        {

            await SendEmail(toEmail, "TaskProject - Password Reset",
                $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                            <div style='max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                                <h1 style='color: #333;'>Password Reset</h1>
                                <p>Hello,</p>
                                <p>We received a request to reset the password for your account. If you did not make this request, you can safely ignore this email.</p>
                                <p>To create a new password, please click the button below:</p>
        
                                <p style='text-align: center; margin: 30px 0;'>
                                    <a href='{_webAppUrl}/reset-password/{token}' 
                                       style='background-color: #007bff; color: white; padding: 15px 25px; text-decoration: none; border-radius: 5px; font-size: 16px;'>
                                       Reset My Password
                                    </a>
                                </p>
        
                                <p>This password reset link will expire in 15 minutes.</p>
                                <p>Sincerely,</p>
                                <p><strong>The LearningTaskProject Team</strong></p>
                            </div>
                        </body>
                    </html>
                    "
                            );
        }

    }
}
