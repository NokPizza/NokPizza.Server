using Microsoft.AspNetCore.Identity;
using NokPizza.Server.Services.Password;

namespace NokPizza.Server.Infrastructure.Password;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public string HashPassword(string password) =>
        _passwordHasher.HashPassword(string.Empty, password);

    public bool VerifyPassword(string passwordHash, string password) =>
        _passwordHasher.VerifyHashedPassword(string.Empty, passwordHash, password)
            is PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
}
