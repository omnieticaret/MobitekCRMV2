using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Entity.Entities
{
    public static class MBCRMRoles
    {
        #region Role Prefixes
        public const string SEO_Role_Prefix = "seo_";

        public const string SEM_Role_Prefix = "sem_";
        #endregion Role Suffixes

        #region Role Suffixes
        public const string Expert_Role_Suffix = "expert";
        #endregion

        #region Role Strings
        public const string Admin_RoleString = "admin";

        public const string Customer_RoleString = "customer";

        public const string Seo_Expert_RoleString = SEO_Role_Prefix + Expert_Role_Suffix;

        public const string Sem_Expert_RoleString = SEM_Role_Prefix + Expert_Role_Suffix;
        #endregion

        #region Role Definitions
        public static readonly Role Admin = new Role()
        {
            RoleType = RoleType.Admin,
            Description = "Admin",
            Name = Admin_RoleString
        };

        public static readonly Role SeoExpert = new Role()
        {
            RoleType = RoleType.Seo,
            Description = "Seo Uzmanı",
            Name = Seo_Expert_RoleString
        };

        public static readonly Role SemExpert = new Role()
        {
            RoleType = RoleType.Sem,
            Description = "Sem Uzmanı",
            Name = Sem_Expert_RoleString
        };

        public static readonly Role Customer = new Role()
        {
            RoleType = RoleType.Customer,
            Description = "Müşteri",
            Name = Customer_RoleString
        };
        #endregion
    }
}


/*
 * 
 */
