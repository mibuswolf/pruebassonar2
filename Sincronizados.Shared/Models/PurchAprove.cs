using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    public class PurchAprove
    {
        public string? PurchId { get; set; }
        public string? Comment { get; set; }
        public string? AxId { get; set; }
        public string? DataAreaId { get; set; }
        public Int64 Refrecid { get; set; }
        public int AXResponse { get; set; }
    }
}
