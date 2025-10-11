namespace Services.Interfaces
{
    public interface IEmailService 
    {
        //Task SendEmail(string toEmail, string subject, string body);
        Task SendPasswordResetEmail(string toEmail, string token);
    }
}
