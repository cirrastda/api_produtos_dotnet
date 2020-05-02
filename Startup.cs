using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CirrasTec.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace CirrasTec.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddXmlSerializerFormatters()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.IgnoreNullValues = true;
                })    
            ;
            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddDbContext<ProdutoContexto>(opt => 
                opt.UseInMemoryDatabase(databaseName:"produtoInMemory")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });
            services.AddApiVersioning();
            services.AddResponseCaching();
            services.AddResponseCompression(opt => {
                opt.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
                opt.EnableForHttps = true;
            }) ;

            services.AddSwaggerGen(c =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var item in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(item.GroupName, new OpenApiInfo { 
                        Title = $"API de Produtos {item.ApiVersion}", 
                        Version = item.ApiVersion.ToString() 
                    });
                }
                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseResponseCaching();
            app.UseResponseCompression();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var item in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json", $"API de Produtos {item.GroupName}");
                }
                c.RoutePrefix = String.Empty;
            });
        }
    }
}
