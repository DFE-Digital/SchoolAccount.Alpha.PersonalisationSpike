using System.Net;
using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.Services
{
    public interface IAcademiesApiService
    {
        Task<AcademyOrganisation?> GetOrganisationDetails(string ukPrn);
        Task<List<AcademyEstablishment>> GetTrustEstablishments(string ukPrn);
        Task<AcademyTrust?> GetTrustDetails(string ukPrn);
    }

    public class AcademiesApiService(HttpClient httpClient) : IAcademiesApiService
    {
        public async Task<AcademyOrganisation?> GetOrganisationDetails(string ukPrn)
        {

            var response = await httpClient.GetAsync($"establishment/{ukPrn}");

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
        public async Task<AcademyTrust?> GetTrustDetails(string ukPrn)
        {

            var response = await httpClient.GetAsync($"trust/{ukPrn}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response.StatusCode + " Could not read organisations", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<AcademyTrust>() ?? null;
        }

        public async Task<List<AcademyEstablishment>> GetTrustEstablishments(string ukPrn)
        {

            var response = await httpClient.GetAsync($"v4/establishments/trust?trustUkprn={ukPrn}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<AcademyEstablishment>();
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response.StatusCode + " Could not read trust's organisations", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<List<AcademyEstablishment>>() ?? new List<AcademyEstablishment>();
        }
    }
}
