namespace WebApplication1
{
    public class UserGroupLdapDto : IEquatable<UserGroupLdapDto>
    {
        public string? UID { get; set; }

        public string? Course { get; set; }

        public string? CurrentState { get; set; }

        public string? Faculty { get; set; }

        public string? GroupName { get; set; }

        public string? Specialty { get; set; }

        public bool Equals(UserGroupLdapDto? other)
        {
            if (other == null) return false;

            return UID == other.UID;
        }

        public override int GetHashCode()
        {
            return UID?.GetHashCode() ?? 0;
        }
    }
}
