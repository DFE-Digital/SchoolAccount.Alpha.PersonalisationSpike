using Microsoft.AspNetCore.WebUtilities;
using SchoolAccount.Alpha.Services.Models;
using System.Net;

namespace SchoolAccount.Alpha.Services
{
    public interface ICollectApiService
    {
        Task<CensusCollectionSummary?> GetSubmissionSummary(string collection, string laestab);
        Task<List<CensusCollectionSummary>> GetSubmissionSummaries(string collection, List<string> laestabs);
        Task<CollectionDetails?> GetCollectionDetails(string collectionName);
    }

    public class CollectApiService(HttpClient httpClient) : ICollectApiService
    {
        public const int ApprovedCode = 7;
        public const string DefaultCollection = "SchoolCensus 2025_Spring";

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

        public async Task<List<CensusCollectionSummary>> GetSubmissionSummaries(string collection, List<string> laestabs)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new("collection", collection),
                new("laestabs", string.Join(",", laestabs))
            };

            var url = QueryHelpers.AddQueryString("Collection/summaries", queryParams!);

            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<CensusCollectionSummary>();
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"{response.StatusCode}: Could not read census status", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<List<CensusCollectionSummary>>() ?? new List<CensusCollectionSummary>();
        }

        public async Task<CollectionDetails?> GetCollectionDetails(string collectionName)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new("collectionName", collectionName)
            };

            var url = QueryHelpers.AddQueryString("Collection/details", queryParams!);

            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"{response.StatusCode}: Could not read collection details", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<CollectionDetails>();
        }
    }
}
