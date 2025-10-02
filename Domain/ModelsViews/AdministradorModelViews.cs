using minimal_api.Domain.Enuns;

namespace minimal_api.Domain.ModelsViews
{
    public record AdministradorModelViews
    {
        public int Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public Perfil Perfil { get; set; } = default!;
    }
}