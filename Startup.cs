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
                cfg.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title="Mimicapi-v1",
                    Version="v1"
                });
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
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "Mimicapi");
                cfg.RoutePrefix = string.Empty;
            });
        }
    }
}
