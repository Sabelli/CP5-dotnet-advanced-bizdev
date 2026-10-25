namespace Biblioteca.Domain.Errors;

public class RequisicaoInvalidaException : Exception
{
    public RequisicaoInvalidaException() { }

    public RequisicaoInvalidaException(string? message) : base(message) { }
}
