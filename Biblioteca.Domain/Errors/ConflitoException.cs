namespace Biblioteca.Domain.Errors;

public class ConflitoException : Exception
{
    public ConflitoException() { }

    public ConflitoException(string? message) : base(message) { }
}
