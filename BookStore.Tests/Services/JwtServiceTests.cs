using BookStore.Models;
using BookStore.Services.Implementations;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace BookStore.Tests.Services
{
    public class JwtServiceTests
    {
        private IConfiguration BuildConfig(
            string? secret = "super-secret-key-long-enough-256bits!",
            string? expiry = "60")
        {
            var dict = new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = secret,
                ["JwtSettings:ExpiryMinutes"] = expiry,
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
        }

        private User MakeUser(string? role = "Admin") => new()
        {
            UserId = 1,
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "hashed",
            RoleNumberNavigation = role != null
                ? new Permrole { PermRole1 = role }
                : null
        };

        [Fact]
        public void GenerateToken_ValidUser_ReturnsNonEmptyToken()
        {
            var svc = new JwtService(BuildConfig());

            var result = svc.GenerateToken(MakeUser());

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GenerateToken_ValidUser_ContainsUsernameClaim()
        {
            var svc = new JwtService(BuildConfig());

            var token = svc.GenerateToken(MakeUser());

            var claims = new JwtSecurityTokenHandler()
                .ReadJwtToken(token)
                .Claims;

            Assert.Contains(claims, c => c.Value == "testuser");
        }

        [Fact]
        public void GenerateToken_UserWithRole_ContainsRoleClaim()
        {
            var svc = new JwtService(BuildConfig());

            var token = svc.GenerateToken(MakeUser("Manager"));

            var claims = new JwtSecurityTokenHandler()
                .ReadJwtToken(token)
                .Claims;

            Assert.Contains(claims, c => c.Value == "Manager");
        }

        [Fact]
        public void GenerateToken_NullRoleNavigation_ClaimFallsBackToGuest()
        {
            var svc = new JwtService(BuildConfig());

            var token = svc.GenerateToken(MakeUser(null));

            var claims = new JwtSecurityTokenHandler()
                .ReadJwtToken(token)
                .Claims;

            Assert.Contains(claims, c => c.Value == "Guest");
        }

        [Fact]
        public void GenerateToken_NullSecret_ThrowsException()
        {
            var svc = new JwtService(BuildConfig(secret: null));

            Assert.ThrowsAny<Exception>(() => svc.GenerateToken(MakeUser()));
        }

        [Fact]
        public void GenerateToken_NullExpiry_ThrowsException()
        {
            var svc = new JwtService(BuildConfig(expiry: null));

            Assert.ThrowsAny<Exception>(() => svc.GenerateToken(MakeUser()));
        }

        [Fact]
        public void GenerateToken_DoesNotExposePasswordInToken()
        {
            var user = MakeUser();
            user.Password = "plaintext-secret-123";

            var token = new JwtService(BuildConfig()).GenerateToken(user);

            Assert.DoesNotContain("plaintext-secret-123", token);
        }

        [Fact]
        public void GenerateToken_TwoDifferentUsers_ProduceDistinctTokens()
        {
            var svc = new JwtService(BuildConfig());

            var u1 = MakeUser();

            var u2 = new User
            {
                UserId = 2,
                UserName = "other",
                FirstName = "O",
                LastName = "T",
                Password = "p",
                RoleNumberNavigation = new Permrole
                {
                    PermRole1 = "Admin"
                }
            };

            Assert.NotEqual(
                svc.GenerateToken(u1),
                svc.GenerateToken(u2));
        }
    }
}