using Sincronizados.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.DTO
{
    public class TokenDTO
    {
        public string Token { get; set; } = null!;
        public Users? User { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime ExpirationLocal => Expiration.ToLocalTime();
    }
}
