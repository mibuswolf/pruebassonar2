using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    public class EmplVacationHistory
    {
        public string TransId { get; set; } = "";
        public DateTime StarDateVacation { get; set; }
        public DateTime EndDateVacation { get; set; }
        public decimal DaysVacation { get; set; } = 0;
    }
}
