﻿namespace TrackingSystem.Api.Shared.Dto.User
{
    public class UserFromLdapDto
    {
        public string? CN { get; set; }

        public string? UID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MiddleName { get; set; }
    }
}
