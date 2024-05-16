namespace WebApplication1
{
    public class UserJobLdapDto : IEquatable<UserJobLdapDto>
    {
        public string? UID { get; set; }

        public string? JobTitle { get; set; }

        public string? JobStake { get; set; }

        public string? EmploymentType { get; set; }

        public string? JobType { get; set; }

        public bool Equals(UserJobLdapDto? other)
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
