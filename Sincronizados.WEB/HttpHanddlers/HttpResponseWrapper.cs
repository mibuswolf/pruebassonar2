using System.Net;

namespace Sincronizados.WEB.HttpHanddlers
{
    //Manejo de llamadas de HTTP
    public class HttpResponseWrapper<T>
    {
        public HttpResponseWrapper(T? response, bool error, HttpResponseMessage httpResponseMessage, string resulstring = "")
        {
            Error = error;
            Response = response;
            HttpResponseMessage = httpResponseMessage;

            if (response is null)
            {
                ResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;

               /// ResultString = result.Result.ToString();
            }
        }

        public bool Error { get; set; }

        public T? Response { get; set; }

        public string ResultString { get; set; }

        public HttpResponseMessage HttpResponseMessage { get; set; }

        public async Task<string?> GetErrorMessageAsync()
        {
            if (!Error)
            {
                return null;
            }

            var statusCode = HttpResponseMessage.StatusCode;
            if (statusCode == HttpStatusCode.NotFound)
            {
                return "Recurso no encontrado";
            }
            else if (statusCode == HttpStatusCode.BadRequest)
            {
                return await HttpResponseMessage.Content.ReadAsStringAsync();
            }
            else if (statusCode == HttpStatusCode.Unauthorized)
            {
                return "Tienes que logearte para hacer esta operación";
            }
            else if (statusCode == HttpStatusCode.Forbidden)
            {
                return "No tienes permisos para hacer esta operación";
            }

            return "Ha ocurrido un error inesperado";
        }
    }
}
