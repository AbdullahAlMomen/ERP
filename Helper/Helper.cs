namespace ERP.Helper
{
    public static class Helper
    {
        public enum UserStatus
        {
            Approved,
            New,
            Unapproved
        }
    }
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
