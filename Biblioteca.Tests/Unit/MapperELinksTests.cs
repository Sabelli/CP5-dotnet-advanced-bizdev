using Biblioteca.Application.Dtos;
using Biblioteca.Application.Mappers;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Tests.Unit;

public class MapperELinksTests
{
    [Fact]
    [Trait("Mapper", "Livro")]
    public void ToLivroDto_DeveMapearNomesELinksSelfUpdateDelete()
    {
        // Arrange
        var livro = new LivroEntity
        {
            Id = 4,
            Titulo = "Dom Casmurro",
            Autor = new AutorEntity { Nome = "Machado" },
            Categoria = new CategoriaEntity { Nome = "Romance" }
        };

        // Act
        var dto = livro.ToLivroDto();

        // Assert
        Assert.Equal("Machado", dto.Autor);
        Assert.Equal("Romance", dto.Categoria);
        Assert.Equal(new[] { "self", "update_livro", "delete_livro" }, dto.Links.Select(l => l.Rel));
        Assert.Equal(new[] { "GET", "PUT", "DELETE" }, dto.Links.Select(l => l.Method));
        Assert.All(dto.Links, l => Assert.Equal("/api/livro/4", l.Href));
    }

    [Fact]
    [Trait("Mapper", "Livro")]
    public void ToLivroDto_SemNavegacoes_DeveUsarTextoVazio()
    {
        var dto = new LivroEntity { Id = 1 }.ToLivroDto();

        Assert.Equal(string.Empty, dto.Autor);
        Assert.Equal(string.Empty, dto.Categoria);
    }

    [Fact]
    [Trait("Mapper", "Emprestimo")]
    public void BuildLinks_Devolvido_NaoDeveTerLinkDevolver()
    {
        var aberto = EmprestimoMapper.BuildLinks(new EmprestimoEntity { Id = 1 });
        var devolvido = EmprestimoMapper.BuildLinks(new EmprestimoEntity { Id = 1, DataDevolucao = DateTime.UtcNow });

        Assert.Contains(aberto, l => l.Rel == "devolver_emprestimo" && l.Method == "PUT");
        Assert.DoesNotContain(devolvido, l => l.Rel == "devolver_emprestimo");
    }

    [Fact]
    [Trait("Mapper", "Emprestimo")]
    public void ToEmprestimoEntity_DeveCalcularDataPrevistaPelosDias()
    {
        var entity = new PostEmprestimoDto { LivroId = 1, UsuarioId = 2, DiasEmprestimo = 10 }.ToEmprestimoEntity();

        Assert.Equal(TimeSpan.FromDays(10), entity.DataPrevistaDevolucao - entity.DataEmprestimo);
        Assert.Null(entity.DataDevolucao);
    }

    [Theory]
    [Trait("Mapper", "PageLinks")]
    [InlineData(0, 10, 25, true, false)]
    [InlineData(10, 10, 25, true, true)]
    [InlineData(20, 10, 25, false, true)]
    [InlineData(0, 10, 5, false, false)]
    public void PageLinks_DeveGerarNextEPrevConformeAPosicao(int desloc, int registros, int total, bool temNext, bool temPrev)
    {
        // Act
        var links = PageLinks.Build("/api/livro", "livro", desloc, registros, total);

        // Assert
        Assert.Contains(links, l => l.Rel == "self");
        Assert.Contains(links, l => l.Rel == "create_livro" && l.Method == "POST");
        Assert.Equal(temNext, links.Any(l => l.Rel == "next"));
        Assert.Equal(temPrev, links.Any(l => l.Rel == "prev"));
    }

    [Fact]
    [Trait("Mapper", "PageLinks")]
    public void PageLinks_Next_DeveAvancarOsRegistrosRetornados()
    {
        var links = PageLinks.Build("/api/livro", "livro", 10, 10, 50);

        Assert.Equal("/api/livro?Deslocamento=20&RegistroRetornado=10", links.First(l => l.Rel == "next").Href);
        Assert.Equal("/api/livro?Deslocamento=0&RegistroRetornado=10", links.First(l => l.Rel == "prev").Href);
    }
}
