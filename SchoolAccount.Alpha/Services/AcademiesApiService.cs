using Microsoft.Extensions.Options;
using SchoolAccount.Alpha.Services.Config;
using System.Net;
using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.Services
{
    public interface IAcademiesApiService
    {
        Task<AcademyOrganisation?> GetOrganisationDetails(string ukPrn);
    }

    public class AcademiesApiService : IAcademiesApiService
    {
        private readonly HttpClient _httpClient;

        public AcademiesApiService(IOptionsSnapshot<AcademiesApiConfig> options)
        {
            var apiConfig = options.Value;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiConfig.PublicUrl),
                DefaultRequestHeaders = { { "ApiKey", apiConfig.ApiKey } }
            };
        }

        public async Task<AcademyOrganisation?> GetOrganisationDetails(string ukPrn)
        {

            var response = await _httpClient.GetAsync($"establishment/{ukPrn}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response.StatusCode + " Could not read organisations", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<AcademyOrganisation>() ?? null;
        }
    }
}
