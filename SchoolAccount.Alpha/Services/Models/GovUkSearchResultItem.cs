using System.Text.Json.Serialization;

namespace SchoolAccount.Alpha.Services.Models;

public class GovUkSearchResultItem
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;
    [JsonPropertyName("public_timestamp")]
    public DateTime? PublicTimestamp { get; set; } 
    [JsonPropertyName("start_date")]
    public DateTime? StartDate { get; set; }  
    [JsonPropertyName("end_date")]
    public DateTime? EndDate { get; set; }
    [JsonPropertyName("indexable_content")]
    public string IndexableContent { get; set; } = string.Empty;
    [JsonPropertyName("link")]
    public string Link { get; set; } = string.Empty;
    [JsonPropertyName("taxons")]
    public List<string> Taxons { get; set; } = new();
    [JsonPropertyName("part_of_taxonomy_tree")]
    public List<string> PartOfTaxonomyTree { get; set; } = new();

}