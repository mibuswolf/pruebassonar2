using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    //JLG 02-10-2023
    //Informacion de las compañias
    public class Companies
    {
        //Codigo de la compañia del AX
        public string CompanyId { get; set; } = null!;
        //Nombre de la compañia del AX
        public string CompanyName { get; set; } = null!;
    }
}
