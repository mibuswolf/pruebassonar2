using Sincronizados.Shared.Models;

namespace Sincronizados.WEB.Auth
{
    public interface ILoginService
    {
        Task LoginAsync(string token);

        Task LogoutAsync();
    }
}
