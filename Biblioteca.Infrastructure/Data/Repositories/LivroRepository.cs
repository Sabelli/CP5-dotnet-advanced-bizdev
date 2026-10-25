using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Data.Repositories;

public class LivroRepository : ILivroRepository
{
    private readonly ApplicationContext _context;

    public LivroRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<LivroEntity> data, int rowsCount)> ObterTodosAsync(int Deslocamento = 0, int RegistroRetornado = 30)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            var total = await _context.Livro.CountAsync();
            var resultado = await _context.Livro
                .AsNoTracking()
                .Include(x => x.Autor)
                .Include(x => x.Categoria)
                .OrderBy(x => x.Id)
                .Skip(Deslocamento)
                .Take(RegistroRetornado)
                .ToListAsync();

            if (resultado.Count == 0)
                throw new NoContentException("Não foram encontrados livros");

            return (resultado, total);
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<LivroEntity> ObterUmAsync(int Id)
    {
        try
        {
            var livro = await _context.Livro
                .AsNoTracking()
                .Include(x => x.Autor)
                .Include(x => x.Categoria)
                .FirstOrDefaultAsync(x => x.Id == Id);
            return livro ?? throw new NoContentException($"Não foi encontrado livro com o id {Id}");
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<LivroEntity> AdicionarAsync(LivroEntity entity)
    {
        try
        {
            _context.Livro.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<LivroEntity> EditarAsync(int Id, LivroEntity entity)
    {
        try
        {
            var livro = await _context.Livro.FirstOrDefaultAsync(x => x.Id == Id)
                ?? throw new NoContentException($"Não foi encontrado livro com o id {Id}");

            livro.Isbn = entity.Isbn;
            livro.Titulo = entity.Titulo;
            livro.AnoPublicacao = entity.AnoPublicacao;
            livro.AutorId = entity.AutorId;
            livro.CategoriaId = entity.CategoriaId;
            await _context.SaveChangesAsync();
            return livro;
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<LivroEntity> DeletarAsync(int Id)
    {
        try
        {
            var livro = await _context.Livro.FirstOrDefaultAsync(x => x.Id == Id)
                ?? throw new NoContentException($"Não foi encontrado livro com o id {Id}");

            _context.Livro.Remove(livro);
            await _context.SaveChangesAsync();
            return livro;
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }
}
