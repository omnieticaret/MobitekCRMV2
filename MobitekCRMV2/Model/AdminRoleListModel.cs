using MobitekCRMV2.Entity.Entities;

namespace MobitekCRMV2.Model
{
    public class AdminRoleListModel
    {
        public List<UserWithRoles> UserList { get; set; }

        public string userType { get; set; }
    }

    public class UserWithRoles
    {
        public User User { get; set; }

        public List<string> Roles { get; set; }
    }
}
