namespace WebApplication1
{
    public class UserLdapDto : IEquatable<UserLdapDto>
    {
        public string? UserName { get; set; }

        public string? UserLogin { get; set; }

        public string? Password { get; set; }

        public string? Group { get; set; }

        public EStatus? Status { get; set; }

        public ERoles Role { get; set; }

        public bool Equals(UserLdapDto? other)
        {
            if (other == null) return false;

            return UserLogin == other.UserLogin;
        }

        public override int GetHashCode()
        {
            return UserLogin?.GetHashCode() ?? 0;
        }
    }
}
