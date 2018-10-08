using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApiNCoreApplication1.Api;
using ApiNCoreApplication1.Entity.UnitofWork;
using ApiNCoreApplication1.Entity.Context;
using ApiNCoreApplication1.Entity.Repository;
using AutoMapper;
using ApiNCoreApplication1.Domain.Mapping;
using ApiNCoreApplication1.Domain.Service;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;


/// <summary>
/// Designed by AnaSoft Inc. 2018
/// http://www.anasoft.net/apincore 
/// 
/// The template version VSIX with test projects included you can get here: http://www.anasoft.net/apincore with added features:
/// -XUnit integration tests project added to the solution (just update the connection string and run tests)
/// -API tests as json file for import to the Postman
/// Another VSIX control can be downloaded to create API .NET Core solution with Dapper ORM implemented instead of Entity Framework and for migration
/// FluentMigrator.Runner is added to created solution.
/// 
/// NOTE:
/// Must update database connection in appsettings.json - "ApiNCoreApplication1.ApiDB"
/// </summary>

namespace ApiNCoreApplication1.Api
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
            //db service
            if (Configuration["ConnectionStrings:UseInMemoryDatabase"] == "True")
                services.AddDbContext<ApiNCoreApplication1Context>(opt => opt.UseInMemoryDatabase("TestDB-" + Guid.NewGuid().ToString()));
            else
                services.AddDbContext<ApiNCoreApplication1Context>(options => options.UseSqlServer(Configuration["ConnectionStrings:ApiNCoreApplication1DB"]));

            //API authentication service
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidAudience = Configuration["Jwt:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        };
                    }
                 );

            // include support for CORS
            // More often than not, we will want to specify that our API accepts requests coming from other origins (other domains). When issuing AJAX requests, browsers make preflights to check if a server accepts requests from the domain hosting the web app. If the response for these preflights don't contain at least the Access-Control-Allow-Origin header specifying that accepts requests from the original domain, browsers won't proceed with the real requests (to improve security).
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy-public",
                    builder => builder.AllowAnyOrigin()   //WithOrigins and define a specific origin to be allowed (e.g. https://mydomain.com)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                .Build());
            });

            //mvc service
            services.AddMvc();

            //general unitofwork injections
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //services injections
            services.AddTransient(typeof(AccountService<,>), typeof(AccountService<,>));
            services.AddTransient(typeof(UserService<,>), typeof(UserService<,>));
            services.AddTransient(typeof(AccountServiceAsync<,>), typeof(AccountServiceAsync<,>));
            services.AddTransient(typeof(UserServiceAsync<,>), typeof(UserServiceAsync<,>));
            //...add other services
            //
            services.AddTransient(typeof(IService<,>), typeof(GenericService<,>));
            services.AddTransient(typeof(IServiceAsync<,>), typeof(GenericServiceAsync<,>));

            //data mapper profiler setting
            Mapper.Initialize((config) =>
            {
                config.AddProfile<MappingProfile>();
            });

        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseMiddleware<ExceptionHandler>();

            app.UseAuthentication(); //needs to be up in the pipeline, before MVC
            app.UseCors("CorsPolicy-public");  //apply to every request
            app.UseMvc();  //must be last line

            //migrations and seeds from json files
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                if (Configuration["ConnectionStrings:UseInMemoryDatabase"] == "False" && !serviceScope.ServiceProvider.GetService<ApiNCoreApplication1Context>().AllMigrationsApplied())
                {
                    if (Configuration["ConnectionStrings:UseMigrationService"] == "True")
                        serviceScope.ServiceProvider.GetService<ApiNCoreApplication1Context>().Database.Migrate();
                }
                //it will seed tables on aservice run from json files if tables empty
                if (Configuration["ConnectionStrings:UseSeedService"] == "True")
                    serviceScope.ServiceProvider.GetService<ApiNCoreApplication1Context>().EnsureSeeded();
            }
        }


    }
}







