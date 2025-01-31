using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProdutoAPI.Models;
using ProdutoAPI.Repositories.Interfaces;
using ProdutoAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProdutoAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _userRepository.FindByUsernameAsync(username)
                ?? throw new UnauthorizedAccessException("Usuário não encontrado");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Senha incorreta");

            return GenerateJwtToken(user);
        }

        public async Task RegisterAsync(string username, string password, string role)
        {
            if (await _userRepository.FindByUsernameAsync(username) != null)
                throw new InvalidOperationException("Usuário já existe");

            // Conversão de string para enum
            if (!Enum.TryParse<UserRole>(role, true, out var userRole))
                throw new ArgumentException("Role inválida. Use 'Admin' ou 'User'.");

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = userRole.ToString() // 👈 Converte o enum para string (ex: "Admin")
            };

            await _userRepository.AddUserAsync(user);
        }

        private string GenerateJwtToken(User user)
        {
            // 1. Criar a identidade de claims
            var identity = new ClaimsIdentity(
                authenticationType: JwtBearerDefaults.AuthenticationScheme,
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role);

            // 2. Adicionar claims básicos
            identity.AddClaims(new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    });

            // 3. Adicionar roles como claims
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));


            // 5. Configurar chave de segurança
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            // 6. Criar descritor do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = identity, // Usar a identidade criada
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            // 7. Gerar e retornar token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
