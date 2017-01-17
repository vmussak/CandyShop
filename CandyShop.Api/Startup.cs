using System;
using System.Threading;
using System.Threading.Tasks;
using CandyShop.Api.Infra.Email;
using CandyShop.Api.Infra.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CandyShop.Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //Unauthorized
            app.Use((context, func) =>
            {
                if (context.Request.Headers.ContainsKey("Authorization") && context.Request.Headers["Authorization"] == "BATATA")
                    return func.Invoke();

                ChangeResponse(context.Response, 401, "Falha de autenticação");
                return Task.CompletedTask;
            });

            //Exception
            app.UseExceptionHandler(builder =>
            {
                builder.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    if (exception != null)
                    {
                        ChangeResponse(context.Response, 500, "Houve uma falha ao executar a operação. Contate o administrador.");
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
                    }

                    return Task.CompletedTask;
                });
            });

            //NotFound
            app.UseStatusCodePages(new StatusCodePagesOptions
            {
                HandleAsync = ctx =>
                {
                    if (ctx.HttpContext.Response.StatusCode == 404)
                        ChangeResponse(ctx.HttpContext.Response, 404, "Rota não encontrada");

                    return Task.CompletedTask;
                }
            });

            app.UseMvc();
        }

        private static void ChangeResponse(HttpResponse response, int code, string content)
        {
            response.Headers.Add("Content-Type", "application/json");
            response.StatusCode = code;
            response.WriteAsync(JsonConvert.SerializeObject(new { Code = code, Content = content }));
        }
    }
}
