using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    //Datos para manejo de la suscripcion y notificacion push web
    public class NotificationSubscription
    {
        public int NotificationSubscriptionId { get; set; }

        public string ?UserId { get; set; }

        public string ?Url { get; set; }

        public string ?P256dh { get; set; }

        public string ?Auth { get; set; }

        public string? VatNum { get; set; }
    }

}
