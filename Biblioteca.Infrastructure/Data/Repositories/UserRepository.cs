using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Errors;
using Biblioteca.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _context;

    public UserRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<UserEntity> AdicionarAsync(UserEntity entity)
    {
        try
        {
            _context.Usuario.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }

    public async Task<UserEntity> ObterPorEmailAsync(string email)
    {
        try
        {
            var user = await _context.Usuario.FirstOrDefaultAsync(x => x.Email == email);
            return user ?? throw new NoContentException("Usuário não encontrado");
        }
        catch (NoContentException ex) { throw new NoContentException(ex.Message); }
        catch (Exception ex) { throw new Exception(ex.Message, ex); }
    }
}
