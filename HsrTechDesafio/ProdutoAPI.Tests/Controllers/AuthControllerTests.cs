using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdutoAPI.Controllers;
using ProdutoAPI.DTOS;
using ProdutoAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdutoAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Login_ValidRequest_ReturnsOkWithToken()
        {
            // Arrange
            _authServiceMock.Setup(x => x.LoginAsync("admin", "senha123"))
                .ReturnsAsync("token_jwt");

            // Act
            var result = await _authController.Login(new LoginDTO
            {
                Username = "admin",
                Password = "senha123"
            });

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_DuplicateUser_ReturnsConflict()
        {
            // Arrange
            _authServiceMock.Setup(x => x.RegisterAsync("admin", "senha123", "Admin"))
                .ThrowsAsync(new InvalidOperationException());

            // Act
            var result = await _authController.Register(new RegisterDTO
            {
                Username = "admin",
                Password = "senha123",
                Role = "Admin"
            });

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}
