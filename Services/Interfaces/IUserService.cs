using Services.DTOs.User;
using Services.Models;
public interface IUserService
{
    Task<UserBasicInfo> GetUserBasicInfo(int id);
    Task AddUser(string username, string email, string password);
    Task UpdateUserInfo(int id, string email, string password);
    Task RemoveUser(int id);
    Task<User?> ValidateLogin(string username, string password);
    Task<bool> ValidateUserEmail(string email);
    Task<bool> ValidateUsername(string username);
    Task UpdatePassword(string email, string password);
    Task PasswordResetRequest(string email);
}

