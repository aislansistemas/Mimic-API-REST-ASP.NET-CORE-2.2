using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mimicapi.Contenxt;
using Mimicapi.v1.Repositories;
using Mimicapi.v1.Repositories.Contracts;
using AutoMapper;
using Mimicapi.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Mimicapi.Helpers.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace Mimicapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public readonly IConfiguration Configuration;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {   //automapper configuração
            var config = new MapperConfiguration(cfg =>
            {
               cfg.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDbContext<MimicContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddApiVersioning(cfg=> { 
                cfg.ReportApiVersions = true;
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddSwaggerGen(cfg =>
            {
                cfg.ResolveConflictingActions(apidescription => apidescription.First());
                cfg.SwaggerDoc("v2.0", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "Mimicapi-v2.0",
                    Version = "v2.0"
                });
                cfg.SwaggerDoc("v1.1", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "Mimicapi-v1.1",
                    Version = "v1.1"
                });
                cfg.SwaggerDoc("v1.0", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title="Mimicapi-v1.0",
                    Version="v1.0"
                });

                var caminhoprojeto = PlatformServices.Default.Application.ApplicationBasePath;
                var nomeprojeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var caminhoxmlcomentario = Path.Combine(caminhoprojeto, nomeprojeto);
                cfg.IncludeXmlComments(caminhoxmlcomentario);

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });
                cfg.OperationFilter<ApiVersionOperationFilter>();


            });
            services.AddScoped<IPalavraRepositoriy, PalavraRepository>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(cfg=>{
                cfg.SwaggerEndpoint("/swagger/v2.0/swagger.json", "Mimicapi - v2.0");
                cfg.SwaggerEndpoint("/swagger/v1.1/swagger.json", "Mimicapi - v1.1");
                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Mimicapi - v1.0");
                cfg.RoutePrefix = string.Empty;
            });
        }
    }
}
