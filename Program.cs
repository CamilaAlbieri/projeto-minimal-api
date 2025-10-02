using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Enuns;
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
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Veiculos

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagem = new List<string>()
    };

     if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validacao.Mensagem.Add("O nome do veiculo não pode ser vazio");
    }

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validacao.Mensagem.Add("A marca do veiculo não pode ficar em branco");
    }

    if (veiculoDTO.Ano < 1950)
    {
        validacao.Mensagem.Add("Veículo muito antigo, aceito somente veículos a partir de 1950");
    }

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoService) =>
{
    

    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagem.Count > 0)
        return Results.BadRequest(validacao);
    

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoService.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

}).WithTags("Veiculos");


app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);

}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO ,IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();

    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagem.Count > 0)
        return Results.BadRequest(validacao);


    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
    
}).WithTags("Veiculos");


#endregion 

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {

    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login feito com sucesso!");
    return Results.Unauthorized();
}).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelViews>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelViews
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = (Perfil)Enum.Parse(typeof(Perfil), adm.Perfil)
        });
    }
    return Results.Ok(adms);
}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscaPorId(id);
    if (administrador == null) return Results.NotFound();
    return Results.Ok(new AdministradorModelViews
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = (Perfil)Enum.Parse(typeof(Perfil), administrador.Perfil)
        });

}).WithTags("Administradores");
 
app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagem = new List<string>()
    };

    if(string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagem.Add("O email do administrador não pode ser vazio");
        
    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagem.Add("A senha não pode ser vazio");
        
    if (administradorDTO.Perfil == null)
        validacao.Mensagem.Add("O perfil do administrador não pode ser vazio");


    if (validacao.Mensagem.Count > 0)
        return Results.BadRequest(validacao);

    var adm = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    administradorServico.Incluir(adm);

    return Results.Created($"/administrador/{adm.Id}", new AdministradorModelViews
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = (Perfil)Enum.Parse(typeof(Perfil), adm.Perfil)
        });
}).WithTags("Administradores");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
