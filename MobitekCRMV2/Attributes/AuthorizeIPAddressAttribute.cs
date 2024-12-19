using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

using ActionExecutingContext = Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;
using ActionFilterAttribute = Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute;

namespace MobitekCRMV2.Attributes
{
    public class AllowedIPAttribute : ActionFilterAttribute
    {

        //overrinding OnActionExecuting method to check Ip, before any code from Action is executed.
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Retrieve user's IP
            string usersIpAddress = filterContext.HttpContext.Connection.LocalIpAddress.ToString();
            var x = filterContext.Result;
            if (!checkIp(usersIpAddress))
            {
                //return 403 Forbidden HTTP code  
                filterContext.Result = new StatusCodeResult(403);
                //forbid //httpcontextresult
            }

            base.OnActionExecuting(filterContext);
        }


        public static bool checkIp(string usersIpAddress)
        {
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            //get allowedIps Setting from Web.Config file and remove whitespaces from int
            string allowedIps = configuration.GetSection("AllowedIPAddresses").Value;

            //convert allowedIPs string to table by exploding string with ';' delimiter
            string[] ips = allowedIps.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //iterate ips table
            foreach (var ip in ips)
            {
                if (ip.Equals(usersIpAddress))
                    return true; //return true confirming that user's address is allowed
            }


            //if we get to this point, that means that user's address is not allowed, therefore returning false
            return false;

        }
    }
}