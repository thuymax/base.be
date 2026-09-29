using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Contexts;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Properties;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Globalization;
using System.Security.Claims;

namespace SEEDONE.SERVICE.Middlewares
{
    /// <summary>
    /// Middleware thiết lập thông tin đầy đủ cho một request
    /// </summary>
    public class AuthenContextHandleMiddleware
    {
        private readonly ILogger _log;
        private readonly RequestDelegate _next;
        public AuthenContextHandleMiddleware(
            ILogger<AuthenContextHandleMiddleware> log,
            RequestDelegate next)
        {
            _log = log;
            _next = next;
        }

        public async Task Invoke(HttpContext context, IContextService contextService)
        {
            var logProperties = new Dictionary<string, string>();
            try
            {
                if (NoRequestAuthentication(context.Request.Path))
                {
                    await _next(context);
                    return;
                }

                var principal = context.User;
                var contextData = BuildContextDataFromPrincipal(principal);

                if (contextData != null)
                {
                    contextService.Set(contextData);
                    await HandleContext(context, true, null);
                }
                else
                {
                    await HandleContext(context, false, "Token không hợp lệ hoặc đã hết hạn.");
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);
                await HandleContext(context, false, ex.Message);
            }
        }

        /// <summary>
        /// Xử lý Context
        /// </summary>
        private async Task HandleContext(HttpContext context, bool isAuthenticated, dynamic data)
        {
            if (isAuthenticated)
            {
                await _next(context);
            }
            else
            {
                // Đăng nhập thất bại
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = new DAResult(401, "Không thể truy cập", data, null);
                var jsonResult = JsonConvert.SerializeObject(result);
                byte[] content = Encoding.UTF8.GetBytes(jsonResult);
                await context.Response.Body.WriteAsync(content, 0, content.Length);
            }
        }

        private ContextData? BuildContextDataFromPrincipal(ClaimsPrincipal? principal)
        {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return null;
            }

            var contextData = new ContextData();

            contextData.Email = GetClaimValue(principal, TokenKeys.Email);

            if (Guid.TryParse(GetClaimValue(principal, TokenKeys.UserId), out var userId))
            {
                contextData.UserId = userId;
            }

            contextData.UserName = GetClaimValue(principal, TokenKeys.UserName);
            contextData.FullName = GetClaimValue(principal, TokenKeys.FullName);
            contextData.PhoneNumber = GetClaimValue(principal, TokenKeys.PhoneNumber);

            if (int.TryParse(GetClaimValue(principal, TokenKeys.Status), out var status))
            {
                contextData.Status = status;
            }

            if (Guid.TryParse(GetClaimValue(principal, TokenKeys.TenantId), out var tenantId))
            {
                contextData.TenantId = tenantId;
            }

            contextData.TenantCode = GetClaimValue(principal, TokenKeys.TenantCode);

            if (Guid.TryParse(GetClaimValue(principal, TokenKeys.DatabaseId), out var databaseId))
            {
                contextData.DatabaseId = databaseId;
            }

            contextData.DatabaseName = GetClaimValue(principal, TokenKeys.DatabaseName);

            return contextData;
        }

        private string? GetClaimValue(ClaimsPrincipal? principal, string claimType)
        {
            return principal?.FindFirst(claimType)?.Value;
        }

        /// <summary>
        /// Các đầu Api không yêu cầu login
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool NoRequestAuthentication(PathString path)
        {
            // Anonymous endpoints are marked with [AllowAnonymous] attribute
            // and handled by the JWT middleware — no manual bypass needed here
            return false;
        }
    }
    

    public static class SetAuthenContextHandlerExtensions
    {
        public static IApplicationBuilder UseSetAuthContextHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenContextHandleMiddleware>();
        }
    }
}
