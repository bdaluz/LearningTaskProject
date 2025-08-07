using Services.Models;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        string CreateToken(User user);
    }
}
