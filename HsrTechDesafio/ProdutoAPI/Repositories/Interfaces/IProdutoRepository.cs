using ProdutoAPI.Models;

namespace ProdutoAPI.Repositories.Interfaces
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetAllProdutosAsync(int userId);
        Task<Produto> GetProdutoByIdAsync(int ProdutoId);
        Task AddProdutoAsync(Produto Produto);
        Task UpdateProdutoAsync(Produto Produto);
        Task DeleteProdutoAsync(int ProdutoId);
    }
}
