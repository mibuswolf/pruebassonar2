using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Enums
{
    public class MyEnums
    {

        public enum VacationType
        {
            Vacation,
            WithPay
        }

        public VacationType MyVacationType { get; set; }
    }
}
