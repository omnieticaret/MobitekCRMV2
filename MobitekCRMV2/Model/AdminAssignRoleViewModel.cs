using MobitekCRMV2.Entity.Entities;

namespace MobitekCRMV2.Model.Models
{
    public class AdminAssignRoleViewModel
    {
        public User User { get; set; }


        public List<string> RoleList { get; set; }
        public List<string> hasRoles { get; set; }
        public List<string> selectedRoles { get; set; }
    }
}
