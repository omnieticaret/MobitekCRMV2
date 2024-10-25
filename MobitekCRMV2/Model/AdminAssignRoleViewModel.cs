using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Entity.Entities;

namespace MobitekCRMV2.Model.Models
{
    public class AdminAssignRoleViewModel
    {
        public UserDto User { get; set; }

        public List<string> RoleList { get; set; }
        public List<string> hasRoles { get; set; }
        public List<string> selectedRoles { get; set; }
    }
}
