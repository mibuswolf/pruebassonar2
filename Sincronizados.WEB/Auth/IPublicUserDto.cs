using Sincronizados.Shared.Models;

namespace Sincronizados.WEB.Auth
{
    public interface IPublicUserDto
    {
        Users CredentialsDTO { get; set; }
    }
}
