using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.ViewModels;

public class CensusTrustViewModel
{
    public string TrustName { get; set; } = string.Empty;
    public List<CensusDetailsViewModel> CensusDetails { get; set; } = new();
    public string TrustUkprn { get; set; } = String.Empty;

    public CensusTrustViewModel()
    {
    }

    public CensusTrustViewModel(AcademyTrust trust, List<CensusCollectionSummary> censusSubmissions)
    {
        TrustName = trust.GiasData?.GroupName ?? "Unknown trust";
        TrustUkprn = trust.GiasData?.Ukprn ?? string.Empty;
        CensusDetails = censusSubmissions
            .Select(c => new CensusDetailsViewModel(c))
            .ToList();
    }
}