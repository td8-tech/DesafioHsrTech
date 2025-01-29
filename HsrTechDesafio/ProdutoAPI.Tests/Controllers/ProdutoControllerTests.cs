using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdutoAPI.Controllers;
using ProdutoAPI.Models;
using ProdutoAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProdutoAPI.Tests.Controllers
{
    public class ProdutoControllerTests
    {
        private readonly Mock<IProdutoService> _ProdutoerviceMock = new();
        private readonly ProdutosController _ProdutoController;

        public ProdutoControllerTests()
        {
            _ProdutoController = new ProdutosController(_ProdutoerviceMock.Object);

            // Simula um usuário autenticado
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _ProdutoController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetAllProduto_Admin_ReturnsAllProduto()
        {
            // Arrange
            _ProdutoerviceMock.Setup(x => x.GetAllProdutosAsync(1, "Admin"))
                .ReturnsAsync(new List<Produto>());

            // Act
            var result = await _ProdutoController.GetAllProdutos();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_NotOwner_ReturnsUnauthorized()
        {
            // Arrange
            _ProdutoerviceMock.Setup(x => x.UpdateProdutoAsync(It.IsAny<Produto>(), 1, "Admin"))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _ProdutoController.UpdateProduto(1, new DTOS.ProdutoDTO
            {
                Nome = "Produto",
                Tipo = "Livro",
                Autor = "Autor"
            });

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
