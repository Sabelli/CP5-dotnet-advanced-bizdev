using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity> AdicionarAsync(UserEntity entity);
    Task<UserEntity> ObterPorEmailAsync(string email);
}
