using MinimalApi.Domain.DTOs;
using MinimalApi.Entities;

namespace MinimalApi.Domain.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
}