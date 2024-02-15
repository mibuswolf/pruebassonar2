using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sincronizados.API.Services;
using Sincronizados.Shared.Models;
using System.Data;
using System.Data.SqlClient;
using WebPush;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Sincronizados.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
   
    public class HRDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conexion;
        private SqlConnectionStringBuilder _builder = new SqlConnectionStringBuilder();
        private readonly IHostingEnvironment env;

        public HRDocumentController(IConfiguration configuration, IHostingEnvironment env) //para ingresar a los valores de configuracion
        {
            _configuration = configuration;
            this.env = env;
        }

        [HttpPost]
        [Route("GetReportPDFRecords")]
        public async Task<string> GetReportPDFRecords([FromBody] Users _users)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            _builder = new SqlConnectionStringBuilder();
            var _listItems = new List<ListValues>();
          
            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_GETVACTIONREPORTPDF", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@EMPLID", SqlDbType.NVarChar, 20).Value = _users.VatNum;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            int numRows = dt.Rows.Count;

                           // JSONString = JsonConvert.SerializeObject(dt);


                            var data = dt.Rows.OfType<DataRow>()
              .Select(row => dt.Columns.OfType<DataColumn>()
                  .ToDictionary(col => col.ColumnName, c => row[c]));

                            return System.Text.Json.JsonSerializer.Serialize(data);

                        }
                    }
                }


                


            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        return "A network-related or instance-specific error occurred while establishing a connection to SQL Server.\"";// Ok("A network-related or instance-specific error occurred while establishing a connection to SQL Server.");
                    default:
                        return ex.Message;//Ok(ex.Message);
                }
            }

           conexion.Close();

        ///    return Ok(JSONString);
        }

        [HttpPost]
        [Route("GetUserListVacRequest")]
        public async Task<IActionResult> GetUserListVacRequest([FromBody] Users _users)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            _builder = new SqlConnectionStringBuilder();
            var _listItems = new List<EmplVacationRequest>();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_USERLISTVACRESQUEST", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@EMPLID", SqlDbType.NVarChar, 20).Value = _users.VatNum;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                _listItems.Add(new EmplVacationRequest
                                {
                                    IdVaction = reader.GetString(0),
                                    VatNum = reader.GetString(1),
                                    DaysVacation = reader.GetDecimal(2),
                                    StarDateVacation = reader.GetDateTime(3),
                                    EndDateVacation = reader.GetDateTime(4),
                                    ResponseInt = reader.GetInt32(5)
                                }); ;
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
        [Route("InsertReqDocument")]
        public async Task<IActionResult> InsertReqDocument([FromBody] HRRequestDocument model)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_INSERTREQDOCUMENT", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@TRANSDATE", SqlDbType.DateTime, 8).Value = DateTime.Today;
                        command.Parameters.Add("@AXUSERID", SqlDbType.NVarChar, 20).Value = model.AxId;
                        command.Parameters.Add("@VATNUM", SqlDbType.NVarChar, 20).Value = model.VATNum;
                        command.Parameters.Add("@DATACOMPANY", SqlDbType.NVarChar, 20).Value = model.DataCompany;
                        command.Parameters.Add("@DOCUTEMPLATE", SqlDbType.NVarChar, 200).Value = model.DocuTemplate;
                        command.Parameters.Add("@DOCUMENTTYPE", SqlDbType.Int).Value = model.DocumentType;
                        command.Parameters.Add("@EMAIL", SqlDbType.NVarChar, 120).Value = model.Email;

                        command.ExecuteNonQuery();
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
            return Ok(model);
        }

        [HttpPost]
        [Route("GetUserListCountRequest")]
        public async Task<IActionResult> GetUserListCountRequest([FromBody] Users _users)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            _builder = new SqlConnectionStringBuilder();
            var _listItems = new List<EmplVacationRequest>();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_USERLISTCOUNTREQUEST", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@EMPLID", SqlDbType.NVarChar, 20).Value = _users.VatNum;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _listItems.Add(new EmplVacationRequest
                                {
                                    VatNum = reader.GetString(0),
                                    DaysVacation = reader.GetDecimal(1)
                                }); ;
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
        [Route("InsertReqVacation")]
        public async Task<IActionResult> InsertReqVacation([FromBody] EmplVacationRequest model)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_INSERTREQVACATION", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@VACATIONDAYS", SqlDbType.Money).Value = model.DaysVacation;
                        command.Parameters.Add("@STARTDATE", SqlDbType.DateTime).Value = model.StarDateVacation.ToString("yyyy-MM-dd");
                        command.Parameters.Add("@ENDDATE", SqlDbType.DateTime).Value = model.EndDateVacation.ToString("yyyy-MM-dd");
                        command.Parameters.Add("@COMMENT_", SqlDbType.NVarChar, 120).Value = model.Comment;
                        command.Parameters.Add("@HCMPERSONNELNUMBERID", SqlDbType.NVarChar, 50).Value = model.VatNum;
                        command.Parameters.Add("@OBGSTATEDOCUMENT", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@RESPONSESTATEDOCUMENT", SqlDbType.Int).Value = 10;
                        command.Parameters.Add("@DATACOMPANY", SqlDbType.NVarChar, 20).Value = model.DataAreaId;
                        command.Parameters.Add("@EMAIL", SqlDbType.NVarChar, 120).Value = model.Email;
                        command.Parameters.Add("@OBGVACATIONTYPE", SqlDbType.Int).Value = model.VacationType;
                        command.Parameters.Add("@VACATIONONHAND", SqlDbType.Money).Value = model.VacationOnHad;
                        command.Parameters.Add("@DocumentDate", SqlDbType.DateTime).Value = DateTime.Today;
                        command.Parameters.Add("@UserName", SqlDbType.NVarChar, 30).Value = model.UserName;
                        command.Parameters.Add("@SuperiorInstance", SqlDbType.NVarChar, 30).Value = model.SuperiorInstance;

                        command.ExecuteNonQuery();
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
            return Ok(model);
        }

        [HttpPost]
        [Route("GetHRListVacAprove")]
        public async Task<IActionResult> GetHRListVacAprove([FromBody] Users _users)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            var _listItems = new List<EmplVacationRequest>();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_GETHRLISTVACAPROVE", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@USERID", SqlDbType.NVarChar, 20).Value = _users.AxId;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _listItems.Add(new EmplVacationRequest
                                {
                                    IdVaction = reader.GetString(0),
                                    Recid = reader.GetInt64(1),
                                    VatNum = reader.GetString(2),
                                    Name = reader.GetString(3),
                                    DaysVacation = reader.GetDecimal(4),
                                    Comment = reader.GetString(5),
                                    RefRecid = reader.GetInt64(8),
                                    StarDateVacation = reader.GetDateTime(9),
                                    EndDateVacation = reader.GetDateTime(10),
                                    VacationOnHad = reader.GetDecimal(11),
                                    AXError = reader.GetInt32(12),
                                    ResponseInt = reader.GetInt32(13)
                                }); ;
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
        [Route("UpdateReqVacation")]
        public async Task<IActionResult> UpdateReqVacation([FromBody] EmplVacationRequest model)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();
            

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_UPDATEREQVACATION", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@REFRECID", SqlDbType.BigInt).Value = model.RefRecid;
                        command.Parameters.Add("@ResponseStateDocument", SqlDbType.Int).Value = model.ResponseInt;
                        command.Parameters.Add("@RejectComment", SqlDbType.NVarChar, 120).Value = model.RejectComment;
                        command.Parameters.Add("@RECID", SqlDbType.BigInt).Value = model.Recid;

                        command.ExecuteNonQuery();
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
            return Ok(model);
        }
        /*
        private async void SendNotificacionClient(EmplVacationRequest model)
        {
            string textoenviado = "";

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();


                //Replace with your generated public/private key
                string publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
                string privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

                //give a website URL or mailto:your-mail-id
                VapidDetails vapidDetails = new VapidDetails("http://mkumaran.net", publicKey, privateKey);
                WebPushClient webPushClient = new WebPushClient();


                var _listItems = new List<NotificationSubscription>();

                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                WorkerService.WriteToFile("vatnum nodel: " + model.VatNum);



                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_GETUSERWEB", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@HCMPERSONNELNUMBERID", SqlDbType.NVarChar, 20).Value = model.VatNum;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _listItems.Add(new NotificationSubscription
                                {
                                    UserId = reader.GetString(0),
                                    Url = reader.GetString(1),
                                    P256dh = reader.GetString(2),
                                    Auth = reader.GetString(3),
                                }); ;
                            }
                        }

                    }
                }

                WorkerService.WriteToFile( "encontrado: " + _listItems.Count.ToString());


                if (_listItems.Count > 0)
                {
                    WorkerService.WriteToFile("url: " + _listItems[0].Url);
                    WorkerService.WriteToFile("P256dh: " + _listItems[0].P256dh);
                    WorkerService.WriteToFile("Auth: " + _listItems[0].Auth);

                    PushSubscription pushSubscription = new PushSubscription(_listItems[0].Url, _listItems[0].P256dh, _listItems[0].Auth);

                    try
                    {
                        WorkerService.WriteToFile("inicia envio: ");

                        if (model.ResponseInt == 30)
                        {
                            textoenviado = "Se ha aprobado su su solicitud #" + model.IdVaction;
                        }

                        textoenviado = "Se ha rechazado su su solicitud #" + model.IdVaction;

                        var payload = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            message = textoenviado,
                            url = "open this URL when user clicks on notification"
                        });

                        await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                    }
                    catch (Exception ex)
                    {
                        WorkerService.WriteToFile("eror: " + ex.Message);
                        Console.Error.WriteLine("Error sending push notification: " + ex.Message);
                        //return -1;
                    }
                }

                conexion.Close();
            }
            catch (SqlException ex)
            {
               
            }
        }

        */
     


    }
}
