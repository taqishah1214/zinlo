namespace Zinlo.Authorization.Roles
{
    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";
        }

        public static class Tenants
        {
            public const string PrimaryAdmin = "Primary Admin";
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string User = "User";
            
        }
    }
}