using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    public class PurchListAprove
    {
        public string? PurchId { get; set; }
        public string? OrderAccount { get; set; }
        public string? PurchName { get; set; }
        public string? Currency { get; set; }
        public string? Comment { get; set; }
        public string? Error { get; set; }
        public string? AxId { get; set; }
        public string? DataAreaId { get; set; }
        public int AppAX { get; set; }
        public Int64 Refrecid { get; set; }
        public decimal PurchTotal { get; set; }
        public DateTime PurchDate { get; set; }
        public string? Requester { get; set; }

    }
}
