using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Data.Repositories;

public class AutorRepository : IAutorRepository
{
    private readonly ApplicationContext _context;

    public AutorRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<AutorEntity> data, int rowsCount)> ObterTodosAsync(int Deslocamento = 0, int RegistroRetornado = 30)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            var total = await _context.Autor.CountAsync();
            var resultado = await _context.Autor.AsNoTracking().OrderBy(x => x.Id).Skip(Deslocamento).Take(RegistroRetornado).ToListAsync();

            if (resultado.Count == 0)
                throw new NoContentException("Não foram encontrados autores");

            return (resultado, total);
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<AutorEntity> ObterUmAsync(int Id)
    {
        try
        {
            var autor = await _context.Autor.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            return autor ?? throw new NoContentException($"Não foi encontrado autor com o id {Id}");
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<AutorEntity> AdicionarAsync(AutorEntity entity)
    {
        try
        {
            _context.Autor.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<AutorEntity> EditarAsync(int Id, AutorEntity entity)
    {
        try
        {
            var autor = await _context.Autor.FirstOrDefaultAsync(x => x.Id == Id)
                ?? throw new NoContentException($"Não foi encontrado autor com o id {Id}");

            autor.Nome = entity.Nome;
            autor.Nacionalidade = entity.Nacionalidade;
            await _context.SaveChangesAsync();
            return autor;
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<AutorEntity> DeletarAsync(int Id)
    {
        try
        {
            var autor = await _context.Autor.FirstOrDefaultAsync(x => x.Id == Id)
                ?? throw new NoContentException($"Não foi encontrado autor com o id {Id}");

            _context.Autor.Remove(autor);
            await _context.SaveChangesAsync();
            return autor;
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }
}
