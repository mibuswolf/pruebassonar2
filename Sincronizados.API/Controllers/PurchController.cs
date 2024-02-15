using Microsoft.AspNetCore.Mvc;
using Sincronizados.Shared.Models;
using System.Data;
using System.Data.SqlClient;

namespace Sincronizados.API.Controllers
{
    public class PurchController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conexion;
        private SqlConnectionStringBuilder _builder;

        public PurchController(IConfiguration configuration) //para ingresar a los valores de configuracion
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("GetPurchListApp")]
        public async Task<IActionResult> GetPurchListApp([FromBody] Users model)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            _builder = new SqlConnectionStringBuilder();
            var _listItems = new List<PurchListAprove>();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_PURCHLISTAPP", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 8).Value = model.DataAreaId;
                        command.Parameters.Add("@USERID", SqlDbType.NVarChar, 16).Value = model.AxId;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _listItems.Add(new PurchListAprove
                                {
                                    PurchId = reader.GetString(0),
                                    OrderAccount = reader.GetString(1),
                                    PurchName = reader.GetString(2),
                                    Currency = reader.GetString(3),
                                    Comment = reader.GetString(4),
                                    Error = reader.GetString(5),
                                    Refrecid = reader.GetInt64(7),
                                    AppAX = reader.GetInt32(8),
                                    PurchTotal = reader.GetDecimal(9),
                                    PurchDate = reader.GetDateTime(10),
                                    Requester = reader.GetString(11)
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
        [Route("InsertAprove")]
        public async Task<IActionResult> InsertAprove([FromBody] PurchAprove model)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection(_configuration["Server:Prod"]);

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("mobi.SP_MOBI_INSERTAPROVE", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //Parametros
                        command.Parameters.Add("@USERID", SqlDbType.NVarChar, 16).Value = model.AxId;
                        command.Parameters.Add("@COMMENT", SqlDbType.NVarChar, 60).Value = model.Comment;
                        command.Parameters.Add("@PURCHID", SqlDbType.NVarChar, 100).Value = model.PurchId;
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 8).Value = model.DataAreaId;
                        command.Parameters.Add("@REFRECID", SqlDbType.BigInt).Value = model.Refrecid;
                        command.Parameters.Add("@RESPONSE", SqlDbType.Int).Value = model.AXResponse;

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
    }
}
