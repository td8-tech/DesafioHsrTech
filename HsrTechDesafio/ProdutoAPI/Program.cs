using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProdutoAPI.Services.Interfaces;
using ProdutoAPI.Services;
using System.Text;
using ProdutoAPI.Repositories.Interfaces;
using ProdutoAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configuração de serviços (ANTES do Build) ----------
// Toda configuração de serviços deve estar aqui

// Adicione os serviços necessários
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure o JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Registre seus repositórios e serviços
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// ---------- Aplicação construída (DEPOIS do Build) ----------
var app = builder.Build(); // A partir daqui, a coleção de serviços é readonly!

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configuração do pipeline (middlewares)
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
