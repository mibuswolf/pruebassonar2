namespace Sincronizados.Shared.Models
{
    public class EmplVacationRequest : EmplVacationHistory
    {
        public string IdVaction { get; set; } = "";
        //  [JsonConverter(typeof(EncryptingJsonConverter), "#my*S3cr3t")]
        public string VatNum { get; set; } = "";
        //  [JsonConverter(typeof(EncryptingJsonConverter), "#my*S3cr3t")]
        public string Name { get; set; } = "";
        // [JsonConverter(typeof(EncryptingJsonConverter), "#my*S3cr3t")]
        public string Email { get; set; } = "";
        public string DataAreaId { get; set; } = "";
        public string Comment { get; set; } = "";
        public int AXError { get; set; }
        public int VacationType { get; set; }
        public decimal VacationOnHad { get; set; }
        public Int64 Recid { get; set; }
        public Int64 RefRecid { get; set; }
        public int ResponseInt { get; set; }
        public string RejectComment { get; set; } = "";
        public string ImageName { get; set; } = "";
        public bool VisibleSend { get; set; } 
        public string UserName { get; set; } = "";
        public int SuperiorInstance { get; set; }
    }
}
