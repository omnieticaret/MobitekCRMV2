using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MobitekCRMV2.Extensions
{
    public static class IdentityExtensions
    {
        public static List<string> Roles(this ClaimsIdentity identity)
        {
            return identity.Claims
                           .Where(c => c.Type == ClaimTypes.Role)
                           .Select(c => c.Value)
                           .ToList();
        }
    }
}
