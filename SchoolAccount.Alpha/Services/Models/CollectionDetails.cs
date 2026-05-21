using System.Text.Json.Serialization;

namespace SchoolAccount.Alpha.Services.Models;

public class CollectionDetails
{
    [JsonPropertyName("dcid")]
    public int Dcid { get; set; }
    [JsonPropertyName("dcName")]
    public string DcName { get; set; } = string.Empty;

    [JsonPropertyName("openDate")]
    public DateTime? OpenDate { get; set; }

    [JsonPropertyName("closeDate")]
    public DateTime? CloseDate { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }
}