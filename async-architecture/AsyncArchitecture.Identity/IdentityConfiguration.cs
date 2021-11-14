using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace AsyncArchitecture.Identity
{
    public class IdentityConfiguration
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new("tasktrackerapi.read"),
                new("tasktrackerapi.write"),
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new()
                {
                    Name = "role",
                    UserClaims = new List<string> { "role" }
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new("tasktrackerapi")
                {
                    DisplayName = "Task Tracker API",
                    Scopes = new List<string> { "tasktrackerapi.read", "tasktrackerapi.write" },
                    ApiSecrets = new List<Secret> { new ("SafePassword".Sha256()) },
                    UserClaims = new List<string> { "role" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new()
                {
                    ClientId = "web-application",
                    ClientName = "Web Application",
                    ClientSecrets = { new Secret("SafePassword".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:5001/signin-oidc" },
                    AllowedCorsOrigins = { "https://localhost:5001" },
                    PostLogoutRedirectUris = { "https://localhost:5001/signout-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "tasktrackerapi.read",
                        "tasktrackerapi.write"
                    },
                    AllowAccessTokensViaBrowser = true
                }
            };

        public static List<TestUser> Users =>
            new()
            {
                new TestUser
                {
                    SubjectId = "123",
                    Username = "TestUser",
                    Password = "Test",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Test User"),
                        new Claim(JwtClaimTypes.Email, "testuser@gmail.com"),
                        new Claim(JwtClaimTypes.Gender, "male"),
                        new Claim(JwtClaimTypes.Role, "admin"),
                    }
                }
            };
    }
}