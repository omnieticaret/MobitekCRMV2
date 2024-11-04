using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Dto.Dtos.NewsSitesDto;
using MobitekCRMV2.Entity.Enums;
using System.Collections.Generic;

namespace MobitekCRMV2.Dto.Dtos.UsersDto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public string Department { get; set; }
        public Status Status { get; set; }
        public List<string> Roles { get; set; }

        public List<ProjectSummaryDto> CustomerProjects { get; set; }
        public List<ProjectSummaryDto> ExpertProjects { get; set; }
        public List<CustomerSummaryDto> Customers { get; set; }
        public UserListDto UserInfo { get; set; }
        public List<NewsSiteDto> NewsSites { get; set; }
    }

    public class UserListDto
    {
        public List<UserSummaryDto> Users { get; set; }
        public int TotalCount { get; set; }
    }

    public class UserSummaryDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public UserType UserType { get; set; }
        public string Department { get; set; }
        public Status Status { get; set; }
        public string Photo { get; set; }
    }

    public class UserCreateUpdateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public string Department { get; set; }
        public Status Status { get; set; }
    }
}