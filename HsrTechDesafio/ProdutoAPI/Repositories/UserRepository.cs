using ProdutoAPI.Models;
using ProdutoAPI.Repositories.Interfaces;
using Dapper;
using System.Data.SqlClient;

namespace ProdutoAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username",
                new { Username = username }
            );
        }

        public async Task AddUserAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@Username, @PasswordHash, @Role)",
                user
            );
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Id = @UserId",
                new { UserId = userId }
            );
        }
    }
}
