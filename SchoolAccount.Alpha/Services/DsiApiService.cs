using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchoolAccount.Alpha.Services.Config;
using SchoolAccount.Alpha.Services.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SchoolAccount.Alpha.Services
{
    public interface IDsiApiService
    {
        Task<List<DsiOrganisation>> GetUserOrganisations(string userId);
    }

    public class DsiApiService(HttpClient httpClient, IOptions<DsiApiConfig> options) : IDsiApiService
    {
        private readonly DsiApiConfig _apiConfig = options.Value;

        private string CreateBearerToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_apiConfig.ApiSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _apiConfig.ServiceAudience,
                Issuer = _apiConfig.Issuer,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<List<DsiOrganisation>> GetUserOrganisations(string userId)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CreateBearerToken());

            var response = await httpClient.GetAsync($"users/{userId}/v2/organisations");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<DsiOrganisation>();
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"{response.StatusCode}: Could not read organisations", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<List<DsiOrganisation>>() ?? new List<DsiOrganisation>();
        }
    }
}
