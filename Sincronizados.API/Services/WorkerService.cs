using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using Sincronizados.Shared.Models;
using WebPush;

namespace Sincronizados.API.Services
{
    public class WorkerService : BackgroundService
    { 
    
        private const int generalDelay = 1 * 10 * 1000; // 10 seconds //300000 5 minutos


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(generalDelay, stoppingToken);
                await DoBackupAsync();
            }
        }

        private static Task DoBackupAsync()
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
                    using (SqlCommand command = new SqlCommand("SELECT IDVACATIONREQUEST,HRDOCNOTIFICATIONWEB,USER_ FROM MicrosoftDynamicsAX.dbo.OBGWorkFlowPending WITH (NOLOCK) WHERE HRDOCNOTIFICATIONWEB = 0", conexion))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var document = reader.GetString(0);
                                var user = reader.GetString(2);

                                SendNotificacionClientPublic(user, document);
                                WriteToFile("Dato actualizado: " + document.ToString());

                                SqlCommand commandUpdate = new SqlCommand("UPDATE MicrosoftDynamicsAX.dbo.OBGWorkFlowPending SET HRDOCNOTIFICATIONWEB = 1 WHERE IDVACATIONREQUEST = '"+ document  + "'", conexion);
                                commandUpdate.ExecuteNonQuery();
                            }
                        }
                    }
                }

                conexion.Close();
            }
            catch (SqlException ex)
            {
                
            }

            return Task.FromResult("Done");
        }

        private static async void SendNotificacionClientPublic(string username, string solicitud)
        {
            string textoenviado = "";
            SqlConnection? conexion;

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                var publicKey = "BIAhKU406gIN53G-zb0jXSUXQpiD514AfP1SXiG0LAt3emhUP-kY5bd965aAIxoBa2NS5y9yMwlg9HbM1iGOKRE";
                var privateKey = "lAQr6GP3BewMfab6iRLjJ89czIgNPiRpvgezz1Vohl8";


                //give a website URL or mailto:your-mail-id
                var vapidDetails = new VapidDetails("http://mkumaran.net", publicKey, privateKey);
                var webPushClient = new WebPushClient();


                var _listItems = new List<NotificationSubscription>();

                builder = new SqlConnectionStringBuilder();
                conexion = new SqlConnection("Data Source=10.0.7.50;Initial Catalog=Desarrollo_CRM_AX;Integrated Security=False;User ID=consultasax;Password=OBGcrmAX17;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True");

                conexion.Open();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SELECT USERNAME,URL,P256DH,AUTH FROM MicrosoftDynamicsAX.dbo.OBGWEBNOTIFICACIONS WHERE USERNAME = '" + username + "'", conexion))
                    {
                        command.CommandType = CommandType.Text;

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

                if (_listItems.Count > 0)
                {
                    var pushSubscription = new PushSubscription(_listItems[0].Url, _listItems[0].P256dh, _listItems[0].Auth);

                    try
                    {
                        textoenviado = "Se le ha asignado la solictud de vacaciones número #" + solicitud + " favor revisar!!";

                        var payload = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            message = textoenviado,
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

                conexion.Close();
            }
            catch (SqlException ex)
            {

            }
        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }

}
