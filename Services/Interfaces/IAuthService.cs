using Services.Models;
using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        string CreateToken(User user);
        Task<RefreshToken> CreateRefreshTokenAsync(int userId);
        Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task DeleteAllUserRefreshTokensAsync(int userId);
        void SetRefreshTokenCookie(string token, HttpContext httpContext);
    }
}
