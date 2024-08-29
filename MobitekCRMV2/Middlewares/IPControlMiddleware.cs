using MobitekCRMV2.Extensions;
using System.Net;

namespace MobitekCRMV2.Middlewares
{
    public class IPControlMiddleware
    {
        private readonly IHttpContextAccessor _accessor;


        readonly RequestDelegate _next;
        IConfiguration _configuration;
        public IPControlMiddleware(RequestDelegate next, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _next = next;
            _accessor = httpContextAccessor; // Dependency Injection:
        }
        public async Task Invoke(HttpContext context)
        {
            //Client'ın IP adresini alıyoruz.
            //bool gettingIp = _accessor.
            //bool gettingIp = context.TryToGetIpAddress(out string ipAddress);
            //Whitelist'te ki tüm IP'leri çekiyoruz.
            var ips = _configuration.GetSection("AllowedIPAddresses")?.Value;
            var IP = _accessor.GetIpAddress();
            var ipList = ips.Split(';');
            //Client IP, whitelist'te var mı kontrol ediyoruz.
            if (!ipList.Contains(IP))
            {
                //Eğer yoksa 403 hatası veriyoruz.
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Bu IP'nin erişim yetkisi yoktur. Current ip = " + IP);
                return;
            }
            await _next.Invoke(context);
        }
        //[HttpGet("GetAllHeaders")]
        //public ActionResult<Dictionary<string, string>> GetAllHeaders()
        //{

        //    Dictionary<string, string> requestHeaders =
        //       new Dictionary<string, string>();
        //    foreach (var header in Request.Headers)
        //    {
        //        requestHeaders.Add(header.Key, header.Value);
        //    }
        //    return requestHeaders;
        //}

        //public static string GetClientIp()
        //{
        //    var ipAddress = string.Empty;

        //    if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
        //        ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        //    else if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null && HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"].Length != 0)
        //        ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
        //    else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
        //        ipAddress = HttpContext.Current.Request.UserHostName;
        //    System.Web.
        //    return ipAddress;
        //}
    }
}