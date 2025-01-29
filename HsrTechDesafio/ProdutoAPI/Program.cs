using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProdutoAPI.Services.Interfaces;
using ProdutoAPI.Services;
using System.Text;
using ProdutoAPI.Repositories.Interfaces;
using ProdutoAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configura��o de servi�os (ANTES do Build) ----------
// Toda configura��o de servi�os deve estar aqui

// Adicione os servi�os necess�rios
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

// Registre seus reposit�rios e servi�os
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// ---------- Aplica��o constru�da (DEPOIS do Build) ----------
var app = builder.Build(); // A partir daqui, a cole��o de servi�os � readonly!

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configura��o do pipeline (middlewares)
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
