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
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<Produto, dynamic, Produto>(
                "SELECT * FROM ProdutoDetails WHERE Id = @ProdutoId",
                (Produto, discriminator) =>
                {
                    // Polimorfismo: Mapeia para Book ou Electronics baseado no Discriminator
                    return Produto.Discriminator switch
                    {
                        "Livro" => new Livro { Autor = discriminator.Author },
                        "Electronicos" => new Electronicos { PeriodoGarantia = discriminator.WarrantyPeriod },
                        _ => Produto
                    };
                },
                new { ProdutoId = ProdutoId },
                splitOn: "Discriminator"
            );

            return result.FirstOrDefault();
        }

        public async Task AddProdutoAsync(Produto Produto)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = Produto switch
            {
                Livro livro => "INSERT INTO Produtos (Name, CreatedByUserId, Discriminator, Author) VALUES (@Name, @CreatedByUserId, 'Book', @Author)",
                Electronicos electronicos => "INSERT INTO Produtos (Name, CreatedByUserId, Discriminator, WarrantyPeriod) VALUES (@Name, @CreatedByUserId, 'Electronics', @WarrantyPeriod)",
                _ => throw new ArgumentException("Tipo de produto inválido")
            };

            await connection.ExecuteAsync(sql, Produto);
        }

        public async Task UpdateProdutoAsync(Produto Produto)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = Produto switch
            {
                Livro livro => "UPDATE Produtos SET Name = @Name, Author = @Author WHERE Id = @Id",
                Electronicos electronicos => "UPDATE Produtos SET Name = @Name, WarrantyPeriod = @WarrantyPeriod WHERE Id = @Id",
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
