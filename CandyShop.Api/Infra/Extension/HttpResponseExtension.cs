using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CandyShop.Api.Infra.Extension
{
    public static class HttpResponseExtension
    {
        public static void End(this HttpResponse response, int code, string content)
        {
            response.Headers.Add("Content-Type", "application/json");
            response.StatusCode = code;
            response.WriteAsync(JsonConvert.SerializeObject(new { Code = code, Content = content }));
        }
    }
}
