namespace WebApplication1
{
    public class UserFromLdapDto : IEquatable<UserFromLdapDto>
    {
        public string? CN { get; set; }

        public string? Password { get; set; }

        public string? UID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MiddleName { get; set; }

        public bool Equals(UserFromLdapDto? other)
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
