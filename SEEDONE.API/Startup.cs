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
using SEEDONE.SERVICE.Config;
using SEEDONE.SERVICE.Interfaces.Service;
using SEEDONE.SERVICE.Service;

namespace SEEDONE.API
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
            services.AddControllers(options =>
            {
                options.Filters.Add<CustomExceptionFilter>();
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SEEDONE.API", Version = "v1" });
            });

            services.AddHttpContextAccessor();

            // Storage configuration
            services.Configure<StorageOptions>(Configuration.GetSection(StorageOptions.SectionName));

            var storageProvider = Configuration.GetValue<string>("Storage:Provider") ?? "local";
            if (storageProvider.Equals("s3", StringComparison.OrdinalIgnoreCase))
                services.AddScoped<IStorageService, S3StorageService>();
            else
                services.AddScoped<IStorageService, LocalStorageService>();

            services.AddScoped<ISerializerService, SerializerService>();
            services.AddScoped<IContextService, WebContextService>();

            var jwtTokenConfig = Configuration.GetSection("JwtConfig").Get<JwtTokenConfig>()
                ?? throw new InvalidOperationException("JwtConfig chưa được cấu hình.");

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

            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    if (allowedOrigins.Length > 0)
                        policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
                    else
                        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SEEDONE.API v1"));
            }
            app.UseHttpsRedirection();
            
            app.UseCors();
            
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                    Configuration.GetValue<string>("Storage:LocalBasePath") ?? "/app/uploads"),
                RequestPath = "/uploads"
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSetAuthContextHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
