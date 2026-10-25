namespace Biblioteca.Domain.Interfaces;

public interface ITokenService
{
    string GerarToken(string email, string role);
}
