using ProdutoAPI.Models;

namespace ProdutoAPI.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> GetAllProdutosAsync(int userId, string role);
        Task<Produto> GetProdutoByIdAsync(int ProdutoId, int userId, string role);
        Task AddProdutoAsync(Produto Produto, int userId);
        Task UpdateProdutoAsync(Produto Produto, int userId, string role);
        Task DeleteProdutoAsync(int ProdutoId, string role, int userId = 2);
    }
}
