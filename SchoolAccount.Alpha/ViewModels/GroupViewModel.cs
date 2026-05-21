using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.ViewModels;

public class GroupViewModel
{
    public List<AcademyEstablishment> Establishments { get; set; } = new();
    public string TrustName { get; set; } = String.Empty;
    public string TrustUkPrn { get; set; } = String.Empty;
    public string CensusSummaryMessage { get; set; } = string.Empty;

    public GroupViewModel()
    {
    }

    public GroupViewModel(AcademyTrust trust, string censusSummaryMessage)
    {
        TrustName = trust.GiasData?.GroupName ?? "Unknown";
        TrustUkPrn = trust.GiasData?.Ukprn ?? "Unknown";
        CensusSummaryMessage = censusSummaryMessage;
        Establishments = trust.Establishments;
    }
}