using Microsoft.AspNet.Identity.EntityFramework;
using MobitekCRMV2.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Authentication
{
    public interface IAuthService
    {
       string GenerateJwtToken(User user, IList<string> roles);
    }
}
