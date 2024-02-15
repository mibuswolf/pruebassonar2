using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sincronizados.Shared.Models
{
    public class HRRequestDocument
    {
        public string DataCompany { get; set; } = "";
        public int DocumentType { get; set; }
        public string DocuTemplate { get; set; } = "";
        public string Email { get; set; } = "";
        public string VATNum { get; set; } = "";
        public string AxId { get; set; } = "";
        public DateTime Processdate { get; set; }
        public DateTime TransDate { get; set; }
        public int AxAprove { get; set; }
        public string ImageName { get; set; } = "";
        public bool IsDateVisible { get; set; }
    }
}
