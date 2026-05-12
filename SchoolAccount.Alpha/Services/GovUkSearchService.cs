using Microsoft.AspNetCore.WebUtilities;
using SchoolAccount.Alpha.Services.Models;
using System.Text.Json;

namespace SchoolAccount.Alpha.Services
{
    public interface IGovUkSearchService
    {
        Task<GovUkSearchResults?> GetLatestDfeDocs(int pageSize, int pageNo);
    }

    public class GovUkSearchService(HttpClient httpClient, ITaxonService taxonService) : IGovUkSearchService
    {
        public async Task<GovUkSearchResults?> GetLatestDfeDocs(int pageSize, int pageNo)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["start"] = ((pageNo - 1) * pageSize).ToString(),
                ["order"] = "-public_timestamp",
                ["filter_organisations"] = "department-for-education",
                ["fields"] = "title,link,public_timestamp,format,taxons,part_of_taxonomy_tree",
                ["count"] = pageSize.ToString()
            };

            var url = QueryHelpers.AddQueryString(String.Empty, queryParams!);
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