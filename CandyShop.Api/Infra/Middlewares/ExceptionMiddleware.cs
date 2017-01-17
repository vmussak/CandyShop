using System;
using System.Threading;
using System.Threading.Tasks;
using CandyShop.Api.Infra.Email;
using CandyShop.Api.Infra.Extension;
using CandyShop.Api.Infra.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace CandyShop.Api.Infra.Middlewares
{
    public static class ExceptionMiddleware
    {
        public static void UseExceptionFilter(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    if (exception == null)
                        return Task.CompletedTask;

                    context.Response.End(500, "Houve uma falha ao executar a operação. Contate o administrador.");
                    new Thread(() =>
                    {
                        try
                        {
                            //Tratar tipos de Exceptions: if (exception is NullReferenceException) { }
                            //Salvar a Exception no Mongo
                            Logger.Write("Exception", exception.ToString());
                            Mail.Send("lenon@engsolutions.com.br", "Falha na API CandyShop", $"Houve uma falha no projeto CandyShop.Api: {exception}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Write("Exception", $"E-Mail: {ex}");
                        }
                    }).Start();

                    return Task.CompletedTask;
                });
            });
        }
    }
}
