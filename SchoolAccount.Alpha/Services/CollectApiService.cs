using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SchoolAccount.Alpha.Services.Config;
using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.Services
{
    public interface ICollectApiService
    {
        Task<CensusCollectionSummary?> GetSubmissionSummary(string collection, string laestab);
    }

    public class CollectApiService(HttpClient httpClient, IOptions<CollectApiConfig> options) : ICollectApiService
    {
        private readonly CollectApiConfig _apiConfig = options.Value;

        public async Task<CensusCollectionSummary?> GetSubmissionSummary(string collection, string laestab)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new("collection", collection),
                new("laestab", laestab)
            };

            var url = QueryHelpers.AddQueryString("Collection/summary", queryParams!);

            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"{response.StatusCode}: Could not read census status", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<CensusCollectionSummary>();
        }
    }
}
