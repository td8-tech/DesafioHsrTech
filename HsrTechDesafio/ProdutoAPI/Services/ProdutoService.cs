using ProdutoAPI.Models;
using ProdutoAPI.Repositories.Interfaces;
using ProdutoAPI.Services.Interfaces;

namespace ProdutoAPI.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository ProdutoRepository)
        {
            _produtoRepository = ProdutoRepository;
        }

        public async Task<IEnumerable<Produto>> GetAllProdutosAsync(int userId, string role)
        {
            return role == "Admin"
                ? await _produtoRepository.GetAllProdutosAsync(0) // 0 = Ignora user ID para admin
                : await _produtoRepository.GetAllProdutosAsync(userId);
        }

        public async Task<Produto> GetProdutoByIdAsync(int ProdutoId, int userId, string role)
        {
            var Produto = await _produtoRepository.GetProdutoByIdAsync(ProdutoId);

            if (role != "Admin" && Produto.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("Acesso negado");

            return Produto;
        }

        public async Task AddProdutoAsync(Produto Produto, int userId)
        {
            Produto.CreatedByUserId = userId; // Define o dono do produto
            await _produtoRepository.AddProdutoAsync(Produto);
        }

        public async Task UpdateProdutoAsync(Produto Produto, int userId, string role)
        {
            await _produtoRepository.UpdateProdutoAsync(Produto);
        }

        public async Task DeleteProdutoAsync(int ProdutoId, string role ,int userId = 2)
        {
            await _produtoRepository.DeleteProdutoAsync(ProdutoId);
        }
    }
}
