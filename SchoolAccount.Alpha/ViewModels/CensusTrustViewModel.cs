namespace SchoolAccount.Alpha.ViewModels;

public class CensusTrustViewModel
{
    public string TrustName { get; set; } = string.Empty;
    public List<CensusDetailsViewModel> CensusDetails { get; set; } = new();
    public string TrustUkprn { get; set; } = String.Empty;
}