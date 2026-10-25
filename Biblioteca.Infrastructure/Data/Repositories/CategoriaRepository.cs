using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Data.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ApplicationContext _context;

    public CategoriaRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<CategoriaEntity> data, int rowsCount)> ObterTodosAsync(int Deslocamento = 0, int RegistroRetornado = 30)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            var total = await _context.Categoria.CountAsync();
            var resultado = await _context.Categoria.AsNoTracking().OrderBy(x => x.Id).Skip(Deslocamento).Take(RegistroRetornado).ToListAsync();

            if (resultado.Count == 0)
                throw new NoContentException("Não foram encontrados categorias");

            return (resultado, total);
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<CategoriaEntity> ObterUmAsync(int Id)
    {
        try
        {
            var categoria = await _context.Categoria.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            return categoria ?? throw new NoContentException($"Não foi encontrado categoria com o id {Id}");
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<CategoriaEntity> AdicionarAsync(CategoriaEntity entity)
    {
        try
        {
            _context.Categoria.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<CategoriaEntity> EditarAsync(int Id, CategoriaEntity entity)
    {
        try
        {
            var categoria = await _context.Categoria.FirstOrDefaultAsync(x => x.Id == Id)
                ?? throw new NoContentException($"Não foi encontrado categoria com o id {Id}");

            categoria.Nome = entity.Nome;
            await _context.SaveChangesAsync();
            return categoria;
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<CategoriaEntity> DeletarAsync(int Id)
    {
        try
        {
            var categoria = await _context.Categoria.FirstOrDefaultAsync(x => x.Id == Id)
                ?? throw new NoContentException($"Não foi encontrado categoria com o id {Id}");

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }
}
