using ProdutoAPI.Models;

namespace ProdutoAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> FindByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task<User> GetUserByIdAsync(int userId);
    }
}
