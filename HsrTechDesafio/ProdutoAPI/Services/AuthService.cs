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
                throw new ArgumentException("Role inválida");

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = userRole
            };

            await _userRepository.AddUserAsync(user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
