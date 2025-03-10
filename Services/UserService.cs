using Microsoft.EntityFrameworkCore;
using ProjetoTasks.Data;
using ProjetoTasks.Models;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProjetoTasks.Services
{
    internal class UserService(ApplicationDbContext Context)
    {
        private async Task<User> GetUsername(string username)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user != null)
            {
                return user;
            }
            throw new InvalidOperationException("\nUser not found.");
        }

        private async Task<User> GetUserByEmail(string email)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                return user;
            }
            throw new InvalidOperationException("\nUser not found.");
        }


        public async Task AddUser(string username, string email, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 15);
            var user = new User(username, email, passwordHash);
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateUserInfo(int id, string email, string password)
        {
            var user = await Context.Users.FindAsync(id);
            if (user != null)
            {
                user.Email = email;
                user.Password = password;
                Context.Update(user);
                await Context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Cannot update details of a non-existent user.");
            }
        }

        public async Task RemoveUser(int id)
        {
            var user = await Context.Users.FindAsync(id);
            if (user != null)
            {
                Context.Users.Remove(user);
                await Context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Cannot remove a user that does not exist.");
            }
        }

        public async Task<User> ValidateLogin(string username, string password)
        {
            var user = await GetUsername(username);
            bool logged = BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password);
            if (logged)
            {
                return user;
            }
            throw new InvalidOperationException("\nIncorrect password.");
        }

        public async Task<bool> ValidateUsername(string username)
        {
            try
            {
                var user = await GetUsername(username);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateUserEmail(string email)
        {
            try
            {
                var user = await GetUserByEmail(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateToken(string email, string token)
        {
            var user = await GetUserByEmail(email);
            if (user.Token == token) {  return true; }
            return false;
        }


        public async Task SendPasswordResetToken(string email)
        {
            var user = await GetUserByEmail(email);
            await CreateToken(user);
            await SendPasswordResetEmail(email, user.Token);
        }

        public async Task CreateToken(User user)
        {
            Guid id = Guid.NewGuid();
            user.Token = id.ToString();
            Context.Update(user);
            await Context.SaveChangesAsync();
        }

        public class Config
        {
            public string Email { get; set; }
            public string SmtpPassword { get; set; }
            public string SmtpServer { get; set; }
            public int SmtpPort { get; set; }
        }

        public async Task<Config> LoadConfig()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectRootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName).FullName;
            string configFilePath = Path.Combine(projectRootPath, "config.json");
            if (File.Exists(configFilePath))
            {
                string json = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<Config>(json);
            }
            throw new Exception("Email config file not found.");
        }

        public async Task SendPasswordResetEmail(string email, string token)
        {
            try
            {
                var config = await LoadConfig();

                string fromEmail = config.Email;
                string toEmail = email;
                string smtpPassword = config.SmtpPassword;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail, "TaskProject");
                    mail.To.Add(toEmail);
                    mail.Subject = "TaskProject - Password Reset";
                    mail.IsBodyHtml = true;
                    mail.Body = @"
                            <html>
                                <body>
                                    <h1>Password Reset</h1>
                                    <p>Enter your token: <strong>" + token + @"</strong></p>
                                    <p>Click <a href='https://github.com/bdaluz/LearningTaskProject'>here</a> to reset your password.</p>
                                    <footer>
                                        <p>LearningTaskProject Team</p>
                                    </footer>
                                </body>
                            </html>";

                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.Port = config.SmtpPort;
                        smtpClient.Credentials = new NetworkCredential(fromEmail, smtpPassword);
                        smtpClient.EnableSsl = true;

                        smtpClient.Send(mail);
                        Console.WriteLine("Password reset email sent!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error to send email: " + ex.Message);
            }
        }

        public async Task UpdatePassword(string email, string password)
        {
            var user = await GetUserByEmail(email);
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 15);
            user.Password = passwordHash;
            Context.Update(user);
            await Context.SaveChangesAsync();
        }


        public async Task<List<User>> GetAllUsers()
        {
            var allUsers = await Context.Users.ToListAsync();
            return allUsers;
        }
    }
}
