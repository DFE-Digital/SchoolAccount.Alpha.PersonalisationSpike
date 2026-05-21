using System.Data.Common;
using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.ViewModels;

public class CensusTrustViewModel
{
    public string TrustName { get; set; } = string.Empty;
    public List<CensusDetailsViewModel> CensusDetails { get; set; } = new();
    public string TrustUkprn { get; set; } = String.Empty;
    public string CollectionName { get; set; } = String.Empty;
    public DateTime? CollectionDueDate { get; set; }
    public DateTime? CollectionOpenDate { get; set; }

    public CensusTrustViewModel()
    {
    }

    public CensusTrustViewModel(AcademyTrust trust, List<CensusCollectionSummary> censusSubmissions, CollectionDetails collectionDetails)
    {
        TrustName = trust.GiasData?.GroupName ?? "Unknown trust";
        TrustUkprn = trust.GiasData?.Ukprn ?? string.Empty;
        CollectionName = collectionDetails.DcName;
        CollectionOpenDate = collectionDetails.OpenDate;
        CollectionDueDate = collectionDetails.DueDate;
        CensusDetails = censusSubmissions
            .Select(c => new CensusDetailsViewModel(c))
            .ToList();
    }

}