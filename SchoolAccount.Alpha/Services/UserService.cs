using System.Security.Claims;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.Services
{
    public static class UserService
    {
        public static SAUser GetUser(IEnumerable<Claim> claims)
        {
            var givenName = claims.Where(c => c.Type == "given_name").Select(c => c.Value).FirstOrDefault() ?? "Unknown";
            var lastName = claims.Where(c => c.Type == "family_name").Select(c => c.Value).FirstOrDefault() ?? "Unknown";
            var dsiId = claims.Where(c => c.Type == "sub").Select(c => c.Value).FirstOrDefault() ?? "Unknown";
            return new SAUser()
            {
                GivenName = givenName,
                LastName = lastName,
                DsiId = dsiId
            };
        }
    }
}
