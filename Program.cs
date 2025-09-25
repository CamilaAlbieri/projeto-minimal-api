using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.ModelsViews;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.Services;
using MinimalApi.Entities;
using MinimalApi.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoService) =>
{

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoService.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

});

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
    
});
#endregion 

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {

    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login feito com sucesso!");
    return Results.Unauthorized();
});

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
