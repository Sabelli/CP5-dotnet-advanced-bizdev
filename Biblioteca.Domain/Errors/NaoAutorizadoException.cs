namespace Biblioteca.Domain.Errors;

public class NaoAutorizadoException : Exception
{
    public NaoAutorizadoException() { }

    public NaoAutorizadoException(string? message) : base(message) { }
}
