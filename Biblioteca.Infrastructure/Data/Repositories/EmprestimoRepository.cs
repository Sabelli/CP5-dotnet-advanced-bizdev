using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Data.Repositories;

public class EmprestimoRepository : IEmprestimoRepository
{
    private readonly ApplicationContext _context;

    public EmprestimoRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<EmprestimoEntity> data, int rowsCount)> ObterTodosAsync(int Deslocamento = 0, int RegistroRetornado = 30)
    {
        try
        {
            if (Deslocamento < 0) Deslocamento = 0;
            if (RegistroRetornado <= 0) RegistroRetornado = 30;

            var total = await _context.Emprestimo.CountAsync();
            var resultado = await _context.Emprestimo
                .AsNoTracking()
                .Include(x => x.Livro)
                .Include(x => x.Usuario)
                .OrderBy(x => x.Id)
                .Skip(Deslocamento)
                .Take(RegistroRetornado)
                .ToListAsync();

            if (resultado.Count == 0)
                throw new NoContentException("Não foram encontrados empréstimos");

            return (resultado, total);
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<EmprestimoEntity> ObterUmAsync(int Id)
    {
        try
        {
            var emprestimo = await _context.Emprestimo
                .AsNoTracking()
                .Include(x => x.Livro)
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == Id);
            return emprestimo ?? throw new NoContentException($"Não foi encontrado empréstimo com o id {Id}");
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<bool> LivroEmprestadoAsync(int LivroId)
    {
        try
        {
            return await _context.Emprestimo.AnyAsync(x => x.LivroId == LivroId && x.DataDevolucao == null);
        }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<EmprestimoEntity> AdicionarAsync(EmprestimoEntity entity)
    {
        try
        {
            _context.Emprestimo.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<EmprestimoEntity> DevolverAsync(int Id)
    {
        try
        {
            var emprestimo = await _context.Emprestimo.FirstOrDefaultAsync(x => x.Id == Id)
                ?? throw new NoContentException($"Não foi encontrado empréstimo com o id {Id}");

            emprestimo.DataDevolucao = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return emprestimo;
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }
}
