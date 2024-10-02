using MobitekCRMV2.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.UserDto.UserDto
{
    public class CreateUserDto
    {
        public string UserName { get; set; } 
        public string Password { get; set; } 
        public string Email { get; set; }    
        public string PhoneNumber { get; set; }
        public string UserType { get; set; } 
        public bool Status { get; set; } 
        public string ErrorMessage { get; set; }
    }

}
