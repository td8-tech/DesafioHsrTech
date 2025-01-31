using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ProdutoAPI.Models;
using ProdutoAPI.Repositories.Interfaces;
using ProdutoAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdutoAPI.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", "SUA_CHAVE_SECRETA_SUPER_SEGURA_AQUI_MINIMO_32_CHARACTERES"}
                })
                .Build();

            _authService = new AuthService(_userRepositoryMock.Object, configuration);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Role = UserRole.Admin.ToString()
            };

            _userRepositoryMock.Setup(x => x.FindByUsernameAsync("admin"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync("admin", "senha123");

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorized()
        {
            // Arrange
            var user = new User { PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123") };
            _userRepositoryMock.Setup(x => x.FindByUsernameAsync("admin")).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync("admin", "senha_errada")
            );
        }

        [Fact]
        public async Task RegisterAsync_DuplicateUsername_ThrowsException()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.FindByUsernameAsync("admin"))
                .ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.RegisterAsync("admin", "senha123", "Admin")
            );
        }
    }
}
