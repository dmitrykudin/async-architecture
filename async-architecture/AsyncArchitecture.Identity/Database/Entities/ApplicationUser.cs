using System;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AsyncArchitecture.Identity.Database.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [JsonIgnore]
        public override string Id { get; set; }

        [JsonIgnore]
        public override string PasswordHash { get; set; }

        [JsonIgnore]
        public override string SecurityStamp { get; set; }

        [JsonIgnore]
        public override string ConcurrencyStamp { get; set; }

        [JsonIgnore]
        public override bool TwoFactorEnabled { get; set; }

        [JsonIgnore]
        public override DateTimeOffset? LockoutEnd { get; set; }

        [JsonIgnore]
        public override bool LockoutEnabled { get; set; }

        [JsonIgnore]
        public override int AccessFailedCount { get; set; }

        public Guid PublicId { get; set; }
    }
}