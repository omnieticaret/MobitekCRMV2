using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System;

namespace MobitekCRMV2.Extensions
{


    public static class HttpContextExtensions
    {
        public static bool TryToGetIpAddress(this HttpContext context, out string ipAddress)
        {
            ipAddress = string.Empty;

            if (!IsRequestAvailable(context))
                return false;

            //if this header not exists try get connection remote IP address
            if (string.IsNullOrEmpty(ipAddress) && context.Connection.RemoteIpAddress != null)
                ipAddress = context.Connection.RemoteIpAddress.ToString();

            //some of the validation
            if (ipAddress != null && ipAddress.Equals(IPAddress.IPv6Loopback.ToString(), StringComparison.InvariantCultureIgnoreCase))
                ipAddress = IPAddress.Loopback.ToString();

            //"TryParse" doesn't support IPv4 with port number
            if (IPAddress.TryParse(ipAddress ?? string.Empty, out var ip))
                //IP address is valid 
                ipAddress = ip.ToString();
            else if (!string.IsNullOrEmpty(ipAddress))
                //remove port
                ipAddress = ipAddress.Split(':').FirstOrDefault();

            return true;
        }


        /// <summary>
        /// Check whether current HTTP request is available
        /// </summary>
        /// <returns>True if available; otherwise false</returns>
        private static bool IsRequestAvailable(HttpContext context)
        {
            if (context == null)
                return false;

            try
            {
                if (context.Request == null)
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
