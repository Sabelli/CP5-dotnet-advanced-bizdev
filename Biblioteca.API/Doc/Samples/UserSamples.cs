using Biblioteca.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Biblioteca.API.Doc.Samples;

public class UserRequestSample : IExamplesProvider<PostUserDto>
{
    public PostUserDto GetExamples() => new()
    {
        Nome = "Maria Silva",
        Email = "maria@teste.com",
        Password = "senha123",
        ConfirmPassword = "senha123",
        Role = "leitor"
    };
}

public class UserResponseSample : IExamplesProvider<GetUserDto>
{
    public GetUserDto GetExamples() => new() { Id = 1, Nome = "Maria Silva", Email = "maria@teste.com", Role = "leitor" };
}

public class AuthRequestSample : IExamplesProvider<PostAuthDto>
{
    public PostAuthDto GetExamples() => new() { Email = "maria@teste.com", Senha = "senha123" };
}

public class AuthResponseSample : IExamplesProvider<GetAuthDto>
{
    public GetAuthDto GetExamples() => new() { Email = "maria@teste.com", Jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." };
}
