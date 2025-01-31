using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProdutoAPI.DTOS;
using ProdutoAPI.Models;
using ProdutoAPI.Services.Interfaces;
using System.Security.Claims;

namespace ProdutoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _ProdutoService;

        public ProdutosController(IProdutoService ProdutoService)
        {
            _ProdutoService = ProdutoService;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<IActionResult> GetAllProdutos()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var Produtos = await _ProdutoService.GetAllProdutosAsync(userId, role);
            return Ok(Produtos);
        }

        // GET: api/Produtos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProdutoById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            try
            {
                var Produto = await _ProdutoService.GetProdutoByIdAsync(id, userId, role);
                return Ok(Produto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Produtos
        [HttpPost]
        public async Task<IActionResult> CreateProduto([FromBody] ProdutoDTO request)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verifique se o usuário está autenticado
            if (User.Identity.IsAuthenticated)
                return Unauthorized(new { message = "Usuário não autenticado." });

            Produto Produto = request.Tipo switch
            {
                "Livro" => new Livro
                {
                    Nome = request.Nome,
                    Autor = request.Autor,
                    CreatedByUserId = request.UserID
                },
                "Eletronicos" => new Electronicos
                {
                    Nome = request.Nome,
                    PeriodoGarantia = request.PeriodoGarantia,
                    CreatedByUserId = request.UserID
                },
                _ => throw new ArgumentException("Tipo de produto inválido")
            };

            await _ProdutoService.AddProdutoAsync(Produto, request.UserID);
            return CreatedAtAction(nameof(GetProdutoById), new { id = Produto.Id }, Produto);
        }

        // PUT: api/Produtos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduto(int id, [FromBody] ProdutoDTO request)
        {
         
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (User.Identity.IsAuthenticated)
                return Unauthorized(new { message = "Usuário não autenticado." });

            Produto Produto = request.Tipo switch
            {
                "Livro" => new Livro
                {
                    Id = id,
                    Nome = request.Nome,
                    Autor = request.Autor
                },
                "Eletronicos" => new Electronicos
                {
                    Id = id,
                    Nome = request.Nome,
                    PeriodoGarantia = request.PeriodoGarantia
                },
                _ => throw new ArgumentException("Tipo de produto inválido")
            };

            try
            {
                await _ProdutoService.UpdateProdutoAsync(Produto, request.UserID, role);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/Produtos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {

            if (User.Identity.IsAuthenticated)
                return Unauthorized(new { message = "Usuário não autenticado." });

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            try
            {
                await _ProdutoService.DeleteProdutoAsync(id, role);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
