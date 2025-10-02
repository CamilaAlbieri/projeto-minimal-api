using minimal_api.Domain.DTOs;
using MinimalApi.Domain.DTOs;
using MinimalApi.Entities;

namespace MinimalApi.Domain.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador Incluir(Administrador administrador);
    Administrador? BuscaPorId(int id);
    List<Administrador> Todos(int? pagina);

    
}