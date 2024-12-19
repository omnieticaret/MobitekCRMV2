using MobitekCRMV2.Authentication;
using MobitekCRMV2.Extensions;
using System.Net;

namespace MobitekCRMV2.Middlewares
{
    public class IPControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IPControlMiddleware(RequestDelegate next, IConfiguration configuration, IHttpContextAccessor accessor, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _configuration = configuration;
            _accessor = accessor;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = _accessor.GetIpAddress();
            var allowedIPs = _configuration.GetSection("AllowedIPAddresses")?.Value;
            var ipList = allowedIPs.Split(';');
            var url = context.Request.Path;

            if (!ipList.Contains(ipAddress))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Bu IP'nin erişim yetkisi yoktur. Current IP = " + ipAddress);
                return;
            }
            if (!url.Value.Contains("/users"))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var tokenHelper = scope.ServiceProvider.GetRequiredService<TokenHelper>();

                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    if (!string.IsNullOrEmpty(authHeader))
                    {
                        var claimsPrincipal = tokenHelper.ValidateToken(_configuration["Jwt:Key"], authHeader);
                        if (claimsPrincipal != null)
                        {
                            context.User = claimsPrincipal;
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            await context.Response.WriteAsync("Geçersiz token1.");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Token eksik2.");
                        return;
                    }
                }
            }

            await _next.Invoke(context);
        }
    }
}