using FluentAssertions;
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
    public class ProdutoServiceTests
    {
        private readonly Mock<IProdutoRepository> _ProdutoRepositoryMock = new();
        private readonly ProdutoService _ProdutoService;

        public ProdutoServiceTests()
        {
            _ProdutoService = new ProdutoService(_ProdutoRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllProdutosAsync_AdminUser_ReturnsAllProdutos()
        {
            // Arrange
            var Produtos = new List<Produto> { new Livro(), new Electronicos() };
            _ProdutoRepositoryMock.Setup(x => x.GetAllProdutosAsync(0)).ReturnsAsync(Produtos);

            // Act
            var result = await _ProdutoService.GetAllProdutosAsync(0, "Admin");

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProdutoByIdAsync_UnauthorizedUser_ThrowsException()
        {
            // Arrange
            var Produto = new Livro { CreatedByUserId = 1 };
            _ProdutoRepositoryMock.Setup(x => x.GetProdutoByIdAsync(1)).ReturnsAsync(Produto);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _ProdutoService.GetProdutoByIdAsync(1, 2, "User")
            );
        }
    }
}
