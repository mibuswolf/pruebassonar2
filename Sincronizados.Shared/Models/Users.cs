using System.ComponentModel.DataAnnotations;


namespace Sincronizados.Shared.Models
{
    //JLG 02-10-2023
    //Informacion del usuario que ingresa al sistema
    public class Users 
    {
        //Nombre de usaurio del dominio
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Username { get; set; } = null!;
        //contraseña del usuario
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Password { get; set; } = null!;
        //Si el usaurio tiene id del ax
        public string? AxId { get; set; }
        //Nombre del usaurio
        public string? NameAlias { get; set; }
        //Compañia relacionada
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string? DataAreaId { get; set; }
        public bool IsEmployee { get; set; } = false;
        //Numero de cedula
        public string? VatNum { get; set; }
        //Correo
        public string? Email { get; set; }
        //Nombre del usaurio del ax si es que lo tiene
        public string? AXRole { get; set; }
        public string? CompanyName { get; set; }
    }


}
