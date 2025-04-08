namespace Services.Interfaces
{
    public interface IAuthService
    {
        string CreateToken(string userId);
    }
}
