using System.Threading.Tasks;
using CandyShop.Api.Infra.Extension;
using Microsoft.AspNetCore.Builder;

namespace CandyShop.Api.Infra.Middlewares
{
    public static class NotFoundMiddleware
    {
        public static void UseNotFound(this IApplicationBuilder app)
        {
            app.UseStatusCodePages(new StatusCodePagesOptions
            {
                HandleAsync = ctx =>
                {
                    if (ctx.HttpContext.Response.StatusCode == 404) { }
                        ctx.HttpContext.Response.End(404, "Rota não encontrada");

                    return Task.CompletedTask;
                }
            });
        }
    }
}
