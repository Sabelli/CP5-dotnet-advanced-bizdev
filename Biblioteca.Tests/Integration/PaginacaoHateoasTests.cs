using System.Net;
using System.Net.Http.Json;
using Biblioteca.Application.Dtos;

namespace Biblioteca.Tests.Integration;

public class PaginacaoHateoasTests : IntegracaoBase
{
    public PaginacaoHateoasTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    [Trait("Integracao", "Paginacao")]
    public async Task Listar_DeveRespeitarRegistroRetornadoEExporLinksNextEPrev()
    {
        // Arrange - garante pelo menos 3 autores
        await CriarAutorAsync();
        await CriarAutorAsync();
        await CriarAutorAsync();

        // Act
        var response = await Client.GetAsync("api/autor?Deslocamento=1&RegistroRetornado=1");
        var pagina = await response.Content.ReadFromJsonAsync<PageResponseDto<GetAutorDto>>(Json);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(pagina!.Items);
        Assert.Equal(1, pagina.Deslocamento);
        Assert.Equal(1, pagina.RegistroRetornado);
        Assert.True(pagina.TotalRegistros >= 3);
        Assert.Contains(pagina.Links, l => l.Rel == "self");
        Assert.Contains(pagina.Links, l => l.Rel == "create_autor" && l.Method == "POST");
        Assert.Equal("/api/autor?Deslocamento=2&RegistroRetornado=1", pagina.Links.First(l => l.Rel == "next").Href);
        Assert.Equal("/api/autor?Deslocamento=0&RegistroRetornado=1", pagina.Links.First(l => l.Rel == "prev").Href);
    }

    [Fact]
    [Trait("Integracao", "Paginacao")]
    public async Task Listar_ParametrosInvalidos_DeveSanitizarPara0e30()
    {
        // Arrange
        await CriarAutorAsync();

        // Act
        var pagina = await Client.GetFromJsonAsync<PageResponseDto<GetAutorDto>>("api/autor?Deslocamento=-10&RegistroRetornado=0", Json);

        // Assert
        Assert.Equal(0, pagina!.Deslocamento);
        Assert.Equal(30, pagina.RegistroRetornado);
    }

    [Fact]
    [Trait("Integracao", "Hateoas")]
    public async Task ObterUm_Livro_DeveTrazerLinksSelfUpdateDelete()
    {
        // Arrange
        var autor = await CriarAutorAsync();
        var categoria = await CriarCategoriaAsync();
        var livro = await CriarLivroAsync(autor.Id, categoria.Id);

        // Act
        var lido = await Client.GetFromJsonAsync<GetLivroDto>($"api/livro/{livro.Id}", Json);

        // Assert
        Assert.Equal(autor.Nome, lido!.Autor);
        Assert.Equal(categoria.Nome, lido.Categoria);
        Assert.Equal(new[] { "self", "update_livro", "delete_livro" }, lido.Links.Select(l => l.Rel));
        Assert.All(lido.Links, l => Assert.Equal($"/api/livro/{livro.Id}", l.Href));
    }

    [Fact]
    [Trait("Integracao", "Hateoas")]
    public async Task Emprestimo_AbertoTemLinkDevolver_DevolvidoNao()
    {
        // Arrange
        var autor = await CriarAutorAsync();
        var categoria = await CriarCategoriaAsync();
        var livro = await CriarLivroAsync(autor.Id, categoria.Id);
        var usuarioId = ObterUsuarioId(EmailAdmin);

        // Act
        var criado = await Client.PostAsJsonAsync("api/emprestimo", new PostEmprestimoDto { LivroId = livro.Id, UsuarioId = usuarioId });
        var emprestimo = (await criado.Content.ReadFromJsonAsync<GetEmprestimoDto>(Json))!;
        RegistrarEmprestimo(emprestimo.Id);

        var devolvido = await Client.PutAsync($"api/emprestimo/{emprestimo.Id}/devolver", null);
        var depois = (await devolvido.Content.ReadFromJsonAsync<GetEmprestimoDto>(Json))!;

        // Assert
        Assert.Equal(HttpStatusCode.Created, criado.StatusCode);
        Assert.Contains(emprestimo.Links, l => l.Rel == "devolver_emprestimo");
        Assert.DoesNotContain(depois.Links, l => l.Rel == "devolver_emprestimo");
    }
}
