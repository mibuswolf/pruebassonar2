using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    //JLG 02-10-2023
    //Informacion del cliente
    public class Customer
    {
        public string AccountNum { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string VatNum { get; set; } = null!;
        public decimal CreditMax { get; set; }
        public string PaymtermId { get; set; } = null!;
        public string DataAreaId { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
