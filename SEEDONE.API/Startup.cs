using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Interfaces.Repo.Business;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.REPO.Repo.Business;
using SEEDONE.SERVICE.Service.Business;
using SEEDONE.SERVICE.Middlewares;
using SEEDONE.SERVICE.Contexts;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SEEDONE.SERVICE.Constants;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SEEDONE.SERVICE.DtoEdit;
using System.Text;
using SEEDONE.SERVICE.Exceptions;

namespace SEEDONE.API
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

            services.AddControllers(options =>
            {
                options.Filters.Add<CustomExceptionFilter>();
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SEEDONE.API", Version = "v1" });
            });

            services.AddHttpContextAccessor();

            services.AddScoped<IDossierService, DossierService>();
            services.AddScoped<IDossierRepo, DossierRepo>();

            services.AddScoped<ITaiLieuGocService, TaiLieuGocService>();
            services.AddScoped<ITaiLieuGocRepo, TaiLieuGocRepo>();

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerRepo, CustomerRepo>();

            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IManufacturerRepo, ManufacturerRepo>();

            services.AddScoped<ICheckerService, CheckerService>();
            services.AddScoped<ICheckerRepo, CheckerRepo>();

            services.AddScoped<IBookmarkTypeService, BookmarkTypeService>();
            services.AddScoped<IBookmarkTypeRepo, BookmarkTypeRepo>();

            services.AddScoped<ITimeSheetService, TimeSheetService>();
            services.AddScoped<ITimeSheetRepo, TimeSheetRepo>(); 
            services.AddScoped<ITypeService, TypeService>();
            services.AddScoped<ISerializerService, SerializerService>();

            services.AddScoped<IContextService, WebContextService>();

            var jwtTokenConfig =
                JsonConvert.DeserializeObject<JwtTokenConfig>(Configuration.GetConnectionString("JwtTokenConfig") ?? string.Empty)
                ?? throw new InvalidOperationException("JwtTokenConfig chưa được cấu hình đúng.");

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.SecretKey));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SEEDONE.API v1"));
            }
            app.UseHttpsRedirection();
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Sử dụng authen context
            app.UseSetAuthContextHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
