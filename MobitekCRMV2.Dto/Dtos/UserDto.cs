using MobitekCRMV2.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public UserType UserType { get; set; }
        public string Status { get; set; }
        public List<ProjectDto> ExpertProjects { get; set; }
    }

    public class ProjectDto
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? Budget { get; set; } 
    }

    public class IndexResponseDto
    {
        public List<UserDto> Users { get; set; }
        public string ErrorMessage { get; set; }
        public UserType UserType { get; set; } 
        public int TotalBudget { get; set; }
    }

}
