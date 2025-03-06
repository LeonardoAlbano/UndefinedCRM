namespace UndefinedCRM.Infrastructure.Security.Cryptography;

public class BcryptAlgorithme
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
}