using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.UsersDto
{
    public class UserDetailDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
        public List<ProjectDetailDto11> ExpertProjects { get; set; }
    }

    public class ProjectDetailDto11
    {
        public string ProjectId { get; set; }
        public string ProjectUrl { get; set; }
        public string Budget { get; set; }
        public string Duration { get; set; }
    }

}
