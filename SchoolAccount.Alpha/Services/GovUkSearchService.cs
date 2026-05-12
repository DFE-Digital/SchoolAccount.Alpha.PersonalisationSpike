using Microsoft.AspNetCore.WebUtilities;
using SchoolAccount.Alpha.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            return await response.Content.ReadFromJsonAsync<GovUkSearchResults>(GetJsonOptions());
        }


        private JsonSerializerOptions GetJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new TaxonListConverter(taxonService));
            return options;
        }
    }

    public class GovUkSearchResults
    {
        public int Total { get; set; }
        public int Start { get; set; }
        public List<GovUkSearchResultItem> Results { get; set; } = new ();
    }

    public class GovUkSearchResultItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("format")]
        public string Format { get; set; } = string.Empty;
        [JsonPropertyName("public_updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;
        [JsonPropertyName("link")]
        public string Link { get; set; } = string.Empty;
        [JsonPropertyName("taxons")]
        public List<string> Taxons { get; set; } = new();
        [JsonPropertyName("part_of_taxonomy_tree")]
        public List<string> PartOfTaxonomyTree { get; set; } = new();

    }

}