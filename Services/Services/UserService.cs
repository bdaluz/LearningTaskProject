using Microsoft.EntityFrameworkCore;
using Services.Data;
using Services.DTOs.User;
using Services.Exceptions;
using Services.Interfaces;
using Services.Models;

namespace Services.Services
{
    public class UserService(ApplicationDbContext Context, IAuthService authService) : IUserService
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

        public async Task<bool> ValidateToken(string email, string token)
        {
            var user = await GetUserByEmail(email);
            if (user == null) return false;
            return user.Token == token;
        }


        public async Task<string> GetPasswordResetToken(string email)
        {
            var user = await GetUserByEmail(email);
            await CreateToken(user);
            return user.Token;
        }

        public async Task CreateToken(User user)
        {
            Guid id = Guid.NewGuid();
            user.Token = id.ToString();
            Context.Update(user);
            await Context.SaveChangesAsync();
        }

        public async Task UpdatePassword(string email, string password)
        {
            var user = await GetUserByEmail(email);
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 11);
            user.Password = passwordHash;
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
