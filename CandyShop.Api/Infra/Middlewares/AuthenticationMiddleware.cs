using System.Threading.Tasks;
using CandyShop.Api.Infra.Extension;
using Microsoft.AspNetCore.Builder;

namespace CandyShop.Api.Infra.Middlewares
{
    public static class AuthenticationMiddleware
    {
        public static void UseAuthentication(this IApplicationBuilder app)
        {
            app.Use((context, func) =>
            {
                if (context.Request.Headers.ContainsKey("Authorization") && context.Request.Headers["Authorization"] == "BATATA")
                    return func.Invoke();

                context.Response.End(401, "Falha de autenticação");
                return Task.CompletedTask;
            });
        }
    }
}
