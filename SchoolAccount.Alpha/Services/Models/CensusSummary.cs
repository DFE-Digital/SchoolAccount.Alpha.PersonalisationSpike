using System.Text.Json.Serialization;

namespace SchoolAccount.Alpha.Services.Models;

public class CensusSummary
{
    [JsonPropertyName("returnStatusCode")]
    public int ReturnStatusCode { get; set; }

    [JsonPropertyName("returnStatus")]
    public string ReturnStatus { get; set; } = string.Empty;

    [JsonPropertyName("collection")]
    public string Collection { get; set; } = string.Empty;

    [JsonPropertyName("laestab")]
    public string Laestab { get; set; } = string.Empty;

    [JsonPropertyName("schoolName")]
    public string SchoolName { get; set; } = string.Empty;

    [JsonPropertyName("errors")]
    public int Errors { get; set; }

    [JsonPropertyName("queries")]
    public int Queries { get; set; }

    [JsonPropertyName("okdErrorsQueries")]
    public int OkdErrorsQueries { get; set; }

    [JsonPropertyName("submittedDate")]
    public DateTime? SubmittedDate { get; set; }

    [JsonPropertyName("approvedDate")]
    public DateTime? ApprovedDate { get; set; }

    [JsonPropertyName("authorisedDate")]
    public DateTime? AuthorisedDate { get; set; }
}