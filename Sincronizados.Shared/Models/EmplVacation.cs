using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    public class EmplVacation
    {
        //Nombre del empleado
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string EmplName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
                              //Dias acumulados
        public decimal RigthDays { get; set; }
        //Dias tomados
        public decimal EndjoyDays { get; set; }
        //Dias disponibles
        public decimal PendingDays { get; set; }
    }
}
