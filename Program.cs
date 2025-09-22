using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Olá !");

app.MapPost("/login", (LoginDTO loginDTO) => {
    {
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login feito com sucesso!");
    }
    return Results.Unauthorized();
});


app.Run();

