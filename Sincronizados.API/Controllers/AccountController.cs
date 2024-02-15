using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Sincronizados.API.Services;
using Sincronizados.Shared.DTO;
using Sincronizados.Shared.Models;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebPush;

namespace Sincronizados.API.Controllers
{
   
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        #region Variables internas
        private readonly IConfiguration _configuration;
        SqlConnection conexion;
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();


        public static List<NotificationSubscription> _subscriptions = new();

        #endregion

        #region Constructor

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;

            if (_subscriptions.Count == 0)
            {
                LoadSubcriptios();
            }
        }

        #endregion

        #region Metodos HTTP GET

        [HttpGet]
        [AllowAnonymous]
        [Route("CompanyList")]
        //Obtiene la lista de compañias activas para hacer el login
        public async Task<IActionResult> CompanyList()
        {
            //variables de conexion a bases de datos
            SqlConnection conexion;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            var _listItems = new List<Companies>();

            try
            {
                //abrimos la conexion
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    //hacemos la llamada al SP
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_LISTCOMPANIES", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                //cargamos los datos en una lista generica
                                _listItems.Add(new Companies
                                {
                                    CompanyId = reader.GetString(0),
                                    CompanyName = reader.GetString(1),
                                });
                            }
                        }
                    }
                }

                conexion.Close();

                return Ok(_listItems);
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        return Ok("A network-related or instance-specific error occurred while establishing a connection to SQL Server.");
                    default:
                        return Ok(ex.Message);
                }
            }
        }


        #endregion

        [HttpPost]
        [AllowAnonymous]
        [Route("subscribe")]
        public int Post(NotificationSubscription notificationSubscription)
        {
            _subscriptions.Add(notificationSubscription);

            builder = new SqlConnectionStringBuilder();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_INSERTUSERWEBLIST", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@USERNAME", SqlDbType.NVarChar, 30).Value = notificationSubscription.UserId;
                        command.Parameters.Add("@NOTIFICATIONSUBSCRIPTIONID", SqlDbType.Int).Value = notificationSubscription.NotificationSubscriptionId;
                        command.Parameters.Add("@URL", SqlDbType.NVarChar, 400).Value = notificationSubscription.Url;
                        command.Parameters.Add("@P256DH", SqlDbType.NVarChar, 400).Value = notificationSubscription.P256dh;
                        command.Parameters.Add("@AUTH", SqlDbType.NVarChar, 400).Value = notificationSubscription.Auth;
                        command.Parameters.Add("@HCMPERSONNELNUMBERID", SqlDbType.NVarChar, 50).Value = notificationSubscription.VatNum;

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                   return -1;
            }
            conexion.Close();

            return _subscriptions.Count();
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("sendall")]
        public async Task<IActionResult> Get()
        {
            //Replace with your generated public/private key
            /*var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";*/

            var publicKey = "BIAhKU406gIN53G-zb0jXSUXQpiD514AfP1SXiG0LAt3emhUP-kY5bd965aAIxoBa2NS5y9yMwlg9HbM1iGOKRE";
            var privateKey = "lAQr6GP3BewMfab6iRLjJ89czIgNPiRpvgezz1Vohl8";


            //give a website URL or mailto:your-mail-id
            var vapidDetails = new VapidDetails("https://crc.obgroup.systems:5559/", publicKey, privateKey);
            var webPushClient = new WebPushClient();

            foreach (var subscription in _subscriptions)
            {
                var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);

                try
                {
                    var payload = JsonSerializer.Serialize(new
                    {
                        message = "this text is from server",
                        url = "open this URL when user clicks on notification"
                    });
                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error sending push notification: " + ex.Message);
                    //return -1;
                }
            }

            return Ok(_subscriptions);
            //return _subscriptions.Count();
        }

        [HttpPost]
        [Route("SendClientPushVacMsg")]
        public  async Task<IActionResult> SendClientPushVacMsg([FromBody] EmplVacationRequest model)
        {
            //Replace with your generated public/private key
            /*var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";*/

            var publicKey = "BIAhKU406gIN53G-zb0jXSUXQpiD514AfP1SXiG0LAt3emhUP-kY5bd965aAIxoBa2NS5y9yMwlg9HbM1iGOKRE";
            var privateKey = "lAQr6GP3BewMfab6iRLjJ89czIgNPiRpvgezz1Vohl8";


            //give a website URL or mailto:your-mail-id
            var vapidDetails = new VapidDetails("https://crc.obgroup.systems:5559/", publicKey, privateKey);
            var webPushClient = new WebPushClient();

            foreach (var subscription in _subscriptions)
            {
                try
                {
                    if (subscription.VatNum == model.VatNum)
                    {
                        var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);


                        string textoenviado;

                        textoenviado = "Se ha rechazado su su solicitud #" + model.IdVaction;

                        if (model.ResponseInt == 30)
                        {
                            textoenviado = "Se ha aprobado su su solicitud #" + model.IdVaction;
                        }

                        var payload = JsonSerializer.Serialize(new
                        {
                            message = textoenviado,
                            url = "open this URL when user clicks on notification"
                        });

                        await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error sending push notification: " + ex.Message);
                    //return -1;
                }
            }
            return Ok(model);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Users _users)
        {
            //Indicamos el dominio en el que vamos a buscar al usuario
            string path = "LDAP://corp.obgroup.com/DC=corp,DC=obgroup,DC=com";

            _users.Password = Base64Decode(_users.Password);

            try
            {
                using (DirectoryEntry entry = new DirectoryEntry(path, _users.Username, _users.Password))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        //Buscamos por la propiedad SamAccountName
                        searcher.Filter = "(samaccountname=" + _users.Username + ")";
                        //Buscamos el usuario con la cuenta indicada
                        var result = searcher.FindOne();

                        if (result != null)
                        {
                            builder = new SqlConnectionStringBuilder();
                            conexion = new SqlConnection(_configuration["Server:Prod"]);

                            conexion.Open();

                            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                            {
                                using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_UserInfo", conexion))
                                {
                                    command.CommandType = CommandType.StoredProcedure;

                                    //Parametros
                                    command.Parameters.Add("@NETWORKALIAS", SqlDbType.NVarChar, 160).Value = _users.Username;

                                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                                    {
                                        while (reader.Read())
                                        {
                                            _users.AxId = reader.GetString(0);//"wvargas";//reader.GetString(0);
                                            _users.NameAlias = reader.GetString(1);
                                            _users.VatNum =  reader.GetString(3);// "107410805";//reader.GetString(3);
                                            _users.Email = reader.GetString(4);
                                            _users.AXRole = "AX";

                                            //_users.Username = "wvargas";
                                            //_users.AxId = "wvargas";
                                            //_users.VatNum = "113740398";
                                        }
                                    }
                                }
                            }

                            //Si el usuario no tine permisos en AX buscamos la informacion del empleado en AX
                            if (string.IsNullOrEmpty(_users.NameAlias))
                            {
                                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                                {
                                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_GETUSERNAME", conexion))
                                    {
                                        command.CommandType = CommandType.StoredProcedure;

                                        //Parametros
                                        command.Parameters.Add("@PERSONNELNUMBER", SqlDbType.NVarChar, 50).Value = _users.VatNum;

                                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                                        {
                                            while (reader.Read())
                                            {
                                                _users.NameAlias = reader.GetString(0);
                                                _users.Email = reader.GetString(1);
                                                _users.AXRole = "User";
                                                _users.AxId = "";
                                            }
                                        }
                                    }
                                }
                            }


                     //       var token = await _userManager.GenerateTwoFactorTokenAsync(, "Email");



                            return Ok(BuildToken(_users));
                        }
                        else
                            return BadRequest("Usuario o contraseña incorrecta!!");
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Dias de vacaciones del usuario
        [HttpPost]
        [Route("VacationList")]
        public async Task<IActionResult> VacationList([FromBody] Users _users)
        {
            var _listItems = new List<EmplVacation>();

            try
            {
                builder = new SqlConnectionStringBuilder();

                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_VACATIONLIST", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 8).Value = _users.DataAreaId;
                        command.Parameters.Add("@EMPLID", SqlDbType.NVarChar, 50).Value = _users.VatNum;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                _listItems.Add(new EmplVacation
                                {
                                    RigthDays = reader.GetDecimal(0),
                                    EndjoyDays = reader.GetDecimal(1),
                                    PendingDays = reader.GetDecimal(2)
                                });
                            }
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        return Ok("A network-related or instance-specific error occurred while establishing a connection to SQL Server.");
                    default:
                        return Ok(ex.Message);
                }
            }

            conexion.Close();

            return Ok(_listItems);
        }

        //Vacaciones asociadas a los colaboradores directos
        [HttpPost]
        [Route("VacationParentList")]
        public async Task<IActionResult> VacationParentList([FromBody] Users _users)
        {
            builder = new SqlConnectionStringBuilder();
            var _listItems = new List<EmplVacation>();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_VACATIONPARENTLIST", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@EMPLID", SqlDbType.NVarChar, 50).Value = _users.VatNum;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                _listItems.Add(new EmplVacation
                                {
                                    EmplName = reader.GetString(1),
                                    PendingDays = reader.GetDecimal(2)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        return Ok("A network-related or instance-specific error occurred while establishing a connection to SQL Server.");
                    default:
                        return Ok(ex.Message);
                }
            }

            conexion.Close();
            return Ok(_listItems);
        }

        [HttpPost]
        [Route("VacationHistory")]
        public async Task<IActionResult> VacationHistory([FromBody] Users _users)
        {
            builder = new SqlConnectionStringBuilder();
            var _listItems = new List<EmplVacationHistory>();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_VACATIONHISTORY", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 8).Value = _users.DataAreaId;
                        command.Parameters.Add("@EMPLID", SqlDbType.NVarChar, 50).Value = _users.VatNum;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                _listItems.Add(new EmplVacationHistory
                                {
                                    TransId = reader.GetString(0),
                                    StarDateVacation = reader.GetDateTime(1),
                                    EndDateVacation = reader.GetDateTime(2),
                                    DaysVacation = reader.GetDecimal(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        return Ok("A network-related or instance-specific error occurred while establishing a connection to SQL Server.");
                    default:
                        return Ok(ex.Message);
                }
            }

            conexion.Close();
            return Ok(_listItems);
        }

        #region Metodos privados
        private TokenDTO BuildToken(Users _users)
        {
            //Metodo para generacion de toques y claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _users.NameAlias),
                new Claim(ClaimTypes.Role, _users.AXRole),
                new Claim(ClaimTypes.WindowsUserClaim, _users.Username),
                new Claim(ClaimTypes.Email, _users.Email),
                new Claim(ClaimTypes.Country, _users.DataAreaId),
                new Claim("VatNum", _users.VatNum),
                new Claim("CompanyName",_users.CompanyName),
                new Claim("AxId",_users.AxId)
            };

            //Informacion del token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials1 = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(90);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials1);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                User = _users,
                Expiration = expiration
            };
        }

        private void LoadSubcriptios()
        {
            SqlConnection? conexion;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection("Data Source=10.0.7.50;Initial Catalog=Desarrollo_CRM_AX;Integrated Security=False;User ID=consultasax;Password=OBGcrmAX17;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True");

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SELECT USERNAME,URL,P256DH,AUTH,HCMPERSONNELNUMBERID FROM MicrosoftDynamicsAX.dbo.OBGWEBNOTIFICACIONS", conexion))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {/*
                                var document = reader.GetString(0);
                                var user = reader.GetString(2);
                                */

                                _subscriptions.Add(new NotificationSubscription() 
                                {
                                    UserId = reader.GetString(0),
                                    Url = reader.GetString(1),
                                    P256dh = reader.GetString(2),
                                    Auth = reader.GetString(3),
                                    VatNum = reader.GetString(4)
                                });
                            }
                        }
                    }
                }



                conexion.Close();
            }
            catch (SqlException ex)
            {

            }
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        #endregion
    }
}
