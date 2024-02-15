using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    public class SuscriptionServices : ISuscriptionServices
    {
        public string? SuscriptionImg { get; set; } = "images/alertOff.png";

        public event Action? SuscriptionServicesChanged;

        public void UpdateSuscription()
        {
            SuscriptionServicesChanged?.Invoke();
        }
    }
}
