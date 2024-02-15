using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    public interface ISuscriptionServices
    {
        event Action SuscriptionServicesChanged;

        string SuscriptionImg { get; set;}

        void UpdateSuscription();
    }
}
