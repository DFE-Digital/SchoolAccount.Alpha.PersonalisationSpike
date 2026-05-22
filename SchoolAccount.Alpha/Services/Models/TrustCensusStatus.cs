using SchoolAccount.Alpha.Services.Models;

public class TrustCensusStatus
{
    public string TrustName { get; set; } = string.Empty;
    public string TrustUkprn { get; set; } = string.Empty;
    public string CollectionName { get; set; } = String.Empty;
    public DateTime? OpenDate { get; set; }
    public DateTime? DueDate { get; set; }
    public List<CensusStatus> CensusStatuses { get; set; } = new();
}