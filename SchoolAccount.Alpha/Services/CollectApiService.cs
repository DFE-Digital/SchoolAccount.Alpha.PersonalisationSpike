using Microsoft.AspNetCore.WebUtilities;
using SchoolAccount.Alpha.Services.Models;
using System.Net;

namespace SchoolAccount.Alpha.Services
{
    public interface ICollectApiService
    {
        Task<CensusSummary?> GetSubmissionSummary(string collection, string laestab);
        Task<List<CensusSummary>> GetSubmissionSummaries(string collection, List<string> laestabs);
        Task<CollectionDetails?> GetCollectionDetails(string collectionName);
        Task<CensusSummary?> GetSchoolCensusStatus(string laestab);
        Task<TrustCensusStatus?> GetTrustCensusStatuses(string laestab);
        Task<string> GetTrustCensusHeadline(List<string> laestabs);
    }

    public class CollectApiService(HttpClient httpClient) : ICollectApiService
    {
        public const int ApprovedCode = 7;
        public const string DefaultCollection = "SchoolCensus 2025_Spring";

        public async Task<CensusSummary?> GetSchoolCensusStatus(string laestab)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new("laestab", laestab)
            };

            var url = QueryHelpers.AddQueryString("census/latest/school", queryParams!);

            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"{response.StatusCode}: Could not read census status", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<CensusSummary>();
        }

        public async Task<TrustCensusStatus?> GetTrustCensusStatuses(string laestab)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new("laestab", laestab)
            };

            var url = QueryHelpers.AddQueryString("census/latest/trust", queryParams!);

            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"{response.StatusCode}: Could not read census status", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<TrustCensusStatus>();
        }

        public async Task<string> GetTrustCensusHeadline(List<string> laestabs)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new("laestabs", string.Join(",", laestabs))
            };

            var url = QueryHelpers.AddQueryString("census/latest/trust/headline", queryParams!);

            var response = await httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"{response.StatusCode}: Could not read census status", response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<HeadlineResponse>() ;
            return result?.Headline ?? "unknown census status";
        }

        public async Task<CensusSummary?> GetSubmissionSummary(string collection, string laestab)
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

            return await response.Content.ReadFromJsonAsync<CensusSummary>();
        }

        public async Task<List<CensusSummary>> GetSubmissionSummaries(string collection, List<string> laestabs)
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
                return new List<CensusSummary>();
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"{response.StatusCode}: Could not read census status", response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<List<CensusSummary>>() ?? new List<CensusSummary>();
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
