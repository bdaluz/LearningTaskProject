using Microsoft.EntityFrameworkCore;
using Services.Data;
using Services.DTOs.User;
using Services.Exceptions;
using Services.Interfaces;
using Services.Models;
using System.Security.Cryptography;

namespace Services.Services
{
    public class UserService(ApplicationDbContext Context, IAuthService authService,
        IEmailService emailService) : IUserService
    {
        private async Task<User?> GetUsername(string username)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        private async Task<User?> GetUserByEmail(string email)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<UserBasicInfo> GetUserBasicInfo(int id)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                return new UserBasicInfo
                {
                    Username = user.Username,
                    Email = user.Email
                };
            }
            throw new InvalidOperationException("User not found.");
        }

        public async Task AddUser(string username, string email, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 11);
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

        public async Task<User?> ValidateLogin(string username, string password)
        {
            var user = await GetUsername(username);
            if (user == null) return null;

            bool logged = BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password);
            return logged ? user : null;
        }

        public async Task<bool> ValidateUsername(string username)
        {
            return await GetUsername(username) != null;
        }

        public async Task<bool> ValidateUserEmail(string email)
        {
            return await GetUserByEmail(email) != null;
        }

        private string GenerateSecureToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }

        private async Task<string> GenerateAndSavePasswordResetToken(User user)
        {
            user.Token = GenerateSecureToken();
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(15);
            Context.Update(user);
            await Context.SaveChangesAsync();
            return user.Token;
        }

        public async Task PasswordResetRequest(string email)
        {
            var user = await GetUserByEmail(email);

            if (user != null)
            {
                string token = await GenerateAndSavePasswordResetToken(user);
                await emailService.SendPasswordResetEmail(email, token);
            }
        }

        public async Task UpdatePassword(string token, string newpassword)
        {
            var user = await Context.Users.SingleOrDefaultAsync(u =>
                u.Token == token &&
                u.ResetTokenExpiresAt > DateTime.UtcNow);

            if (user == null)
            {
                throw new InvalidOperationException("Invalid Token.");
            }

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(newpassword, 11);
            user.Password = passwordHash;
            user.Token = null;
            user.ResetTokenExpiresAt = null;
            Context.Update(user);
            await Context.SaveChangesAsync();
            await authService.DeleteAllUserRefreshTokensAsync(user.Id);
        }


        public async Task<List<User>> GetAllUsers()
        {
            var allUsers = await Context.Users.ToListAsync();
            return allUsers;
        }
    }
}
