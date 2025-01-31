using ProdutoAPI.Models;
using System.Data.SqlClient;
using Dapper;
using ProdutoAPI.Repositories.Interfaces;

namespace ProdutoAPI.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly string _connectionString;

        public ProdutoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Produto>> GetAllProdutosAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Produto>(
                "EXEC GetUserProdutos @UserId",
                new { UserId = userId },
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

        public async Task<Produto> GetProdutoByIdAsync(int ProdutoId)
        {
            const string query = @"
        SELECT 
            Id, 
            Nome, 
            CreatedByUserId, 
            Discriminator,  -- Coluna que define o tipo
            Autor,         -- Específico de livro
            PeriodoGarantia  -- Específico de Eletronicos
        FROM Produtos 
        WHERE Id = @ProductId";

            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<Produto, dynamic, Produto>(
                query,
                (product, discriminator) => product switch
                {
                    Livro book when discriminator.Discriminator == "Livro" =>
                        new Livro { Autor = discriminator.Autor },
                    Electronicos electronics when discriminator.Discriminator == "Electronicos" =>
                        new Electronicos { PeriodoGarantia = discriminator.PeriodoGarantia },
                    _ => throw new InvalidOperationException("Tipo de produto desconhecido")
                },
                new { ProductId = ProdutoId },
                splitOn: "Discriminator"  // Coluna que separa propriedades base/específicas
            );

            return result.FirstOrDefault();
        }

        public async Task AddProdutoAsync(Produto Produto)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = Produto switch
            {
                Livro livro => "INSERT INTO Produtos (Nome, CreatedByUserId, Discriminator, Autor) VALUES (@Nome, @CreatedByUserId, 'Livro', @Autor)",
                Electronicos electronicos => "INSERT INTO Produtos (Nome, CreatedByUserId, Discriminator, PeriodoGarantia) VALUES (@Nome, @CreatedByUserId, 'Electronicos', @PeriodoGarantia)",
                _ => throw new ArgumentException("Tipo de produto inválido")
            };

            await connection.ExecuteAsync(sql, Produto);
        }

        public async Task UpdateProdutoAsync(Produto Produto)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = Produto switch
            {
                Livro livro => "UPDATE Produtos SET Nome = @Nome, Autor = @Autor WHERE Id = @Id",
                Electronicos electronicos => "UPDATE Produtos SET Nome = @Nome, PeriodoGarantia = @PeriodoGarantia WHERE Id = @Id",
                _ => throw new ArgumentException("Tipo de produto inválido")
            };

            await connection.ExecuteAsync(sql, Produto);
        }

        public async Task DeleteProdutoAsync(int ProdutoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "DELETE FROM Produtos WHERE Id = @ProdutoId",
                new { ProdutoId = ProdutoId }
            );
        }
    }
}
