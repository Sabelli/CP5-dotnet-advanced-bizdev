namespace Biblioteca.Domain.Errors;

public class ServicoExternoException : Exception
{
    public ServicoExternoException() { }

    public ServicoExternoException(string? message) : base(message) { }
}
