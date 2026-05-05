using System.Text.Json.Serialization;

namespace SchoolAccount.Alpha.Services.Models;

public class DsiOrganisation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public NameAndIdStringField Category { get; set; } = new();

    [JsonPropertyName("urn")]
    public string Urn { get; set; } = string.Empty;

    [JsonPropertyName("ukprn")]
    public string Ukprn { get; set; } = string.Empty;

    [JsonPropertyName("establishmentNumber")]
    public string EstablishmentNumber { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public NameAndIdNoField Status { get; set; } = new();

    [JsonPropertyName("type")]
    public NameAndIdStringField Type { get; set; } = new();

    [JsonPropertyName("localAuthority")]
    public NameAndIdStringField LocalAuthority { get; set; } = new();

    [JsonPropertyName("closedOn")]
    public DateTimeOffset? ClosedOn { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    public string PercentageFsm { get; set; } = string.Empty;
}

public class NameAndIdStringField
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class NameAndIdNoField
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
