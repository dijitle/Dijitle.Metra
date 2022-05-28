using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.IO;
using Dijitle.Metra.API.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Prometheus;
using Microsoft.Extensions.Hosting;

namespace Dijitle.Metra.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddMvcCore().AddApiExplorer();
            services.AddHttpClient("GTFSClient", client =>
            {
                client.BaseAddress = new Uri(Configuration["gtfsURL"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Configuration["accesskey"]}:{Configuration["secretkey"]}")));
            })
            .ConfigurePrimaryHttpMessageHandler(handler =>
            new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dijitle Metra API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
            });

            services.AddSingleton<IMetraService, MetraService>();
            services.AddSingleton<IGTFSService, GTFSService>();

            services.AddHealthChecks();
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", b =>
                {
                    b.WithOrigins(Configuration.GetSection("CORsURLs").Get<string[]>())
                     .AllowAnyHeader()
                     .AllowAnyMethod();
                });
            });

            services.AddHostedService<DataRefresher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowOrigin");
            app.UseRouting();
            app.UseStaticFiles();

            app.UseHttpMetrics();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dijitle Metra API V1");
                c.RoutePrefix = "api";
                c.EnableDeepLinking();
                c.EnableFilter();
                c.InjectJavascript("https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js");
                c.InjectJavascript("/js/swagger.js");
                c.InjectStylesheet("/css/swagger.css");
            });

            app.UseEndpoints(e => {
                e.MapControllers();
                e.MapRazorPages();
                e.MapMetrics();
            });

            app.UseHealthChecks("/health");
        }
    }
}
