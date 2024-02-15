using Sincronizados.Shared.Models;

namespace Sincronizados.WEB.Auth
{
    public class PublicUserDto : IPublicUserDto
    {
        public Users CredentialsDTO { get; set; } = new Users();
    }
}
