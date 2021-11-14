using System;
using System.Linq;
using System.Security.Claims;
using AsyncArchitecture.Identity.Database;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AsyncArchitecture.Identity
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlite(connectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });
            services.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlite(connectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                context.Database.Migrate();

                EnsureSeedData(context);

                var ctx = scope.ServiceProvider.GetService<ApplicationDbContext>();
                ctx.Database.Migrate();
                EnsureUsers(scope);
            }
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var testUser = userMgr.FindByNameAsync("TestUser").Result;
            if (testUser == null)
            {
                testUser = new IdentityUser
                {
                    UserName = "TestUser",
                    Email = "testuser@gmail.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(testUser, "Test123!").Result;
                if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

                result = userMgr.AddClaimsAsync(testUser, new[]
                {
                    new Claim(JwtClaimTypes.Name, "Test User"),
                    new Claim(JwtClaimTypes.GivenName, "Test"),
                    new Claim(JwtClaimTypes.FamilyName, "User"),
                    new Claim(JwtClaimTypes.WebSite, "http://test.com")
                }).Result;
                if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

                Log.Debug("TestUser created");
            }
            else
            {
                Log.Debug("TestUser already exists");
            }
        }


        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Log.Debug("Clients being populated");
                foreach (var client in IdentityConfiguration.Clients.ToList()) context.Clients.Add(client.ToEntity());

                context.SaveChanges();
            }
            else
            {
                Log.Debug("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in IdentityConfiguration.IdentityResources.ToList())
                    context.IdentityResources.Add(resource.ToEntity());

                context.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Log.Debug("ApiScopes being populated");
                foreach (var resource in IdentityConfiguration.ApiScopes.ToList()) context.ApiScopes.Add(resource.ToEntity());

                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }

            if (!context.ApiResources.Any())
            {
                Log.Debug("ApiResources being populated");
                foreach (var resource in IdentityConfiguration.ApiResources.ToList()) context.ApiResources.Add(resource.ToEntity());

                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }
        }
    }
}