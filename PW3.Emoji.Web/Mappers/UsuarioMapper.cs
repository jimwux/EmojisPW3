using PW3.Emoji.Entidades.EF;
using PW3.Emoji.Web.Models;
namespace PW3.Emoji.Web.Mappers;

public static class UsuarioMapper
{
    public static Usuario ToEntity(this UsuarioViewModel viewModel)
    {
        return new Usuario
        {
            Email = viewModel.Email,
            HashPassword = viewModel.HashPassword,
            Nombre = viewModel.Nombre,
            RolId = viewModel.RolId
        };
    }
}
