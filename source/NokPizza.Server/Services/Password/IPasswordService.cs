namespace NokPizza.Server.Services.Password;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string passwordHash, string password);
}
