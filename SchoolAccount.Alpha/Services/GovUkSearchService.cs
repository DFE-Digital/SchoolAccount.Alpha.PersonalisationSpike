using Microsoft.AspNetCore.WebUtilities;
using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.Services
{
    public interface IGovUkSearchService
    {
        Task<GovUkSearchResults?> GetLatestDfeDocs(int pageSize, int pageNo);
        Task<GovUkSearchResults?> SearchDfeDocs(int pageSize, int pageNo, string query);
    }

    public class GovUkSearchService(HttpClient httpClient, ITaxonService taxonService) : IGovUkSearchService
    {
        private readonly List<string> StandardFields =
            ["title",
                "description",
                "link",
                "public_timestamp",
                "format",
                "taxons",
                "part_of_taxonomy_tree",
                "start_date",
                "end_date",
                //"indexable_content"
            ];

        public async Task<GovUkSearchResults?> GetLatestDfeDocs(int pageSize, int pageNo)
        {
            var url = BuildQueryParams(pageSize, pageNo);

            var response = await httpClient.GetAsync(url);

            return await DeserializeResponse(response);
        }

        private string BuildQueryParams(int pageSize, int pageNo, string? query = null)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new("start", ((pageNo - 1) * pageSize).ToString()),
                new("filter_organisations", "department-for-education"),
                new("filter_any_format", "detailed_guide"),
                new("filter_any_format", "guidance"),
                new("fields", string.Join(',', StandardFields)),
                new("count", pageSize.ToString())
            };

            if (string.IsNullOrEmpty(query))
            {
                queryParams.Add(new("order", "-public_timestamp"));
            }
            else
            {
                queryParams.Add(new("q", query));
            }

            return QueryHelpers.AddQueryString(string.Empty, queryParams!);
        }

        public async Task<GovUkSearchResults?> SearchDfeDocs(int pageSize, int pageNo, string query)
        {
            var url = BuildQueryParams(pageSize, pageNo, query);
            var response = await httpClient.GetAsync(url);

            return await DeserializeResponse(response);
        }

        private async Task<GovUkSearchResults?> DeserializeResponse(HttpResponseMessage response)
        {

            var results = await response.Content.ReadFromJsonAsync<GovUkSearchResults>();

            // Post-process to convert taxon IDs to names
            if (results?.Results != null)
            {
                foreach (var item in results.Results)
                {
                    item.Taxons = item.Taxons
                        .Select(taxonService.GetTaxonName)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList();

                    item.PartOfTaxonomyTree = item.PartOfTaxonomyTree
                        .Select(taxonService.GetTaxonName)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList();
                }
            }

            return results;
        }
    }
}