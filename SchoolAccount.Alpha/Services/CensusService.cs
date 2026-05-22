using SchoolAccount.Alpha.Services;
using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.Services
{
    public interface ICensusService
    {
        Task<TrustCensusStatus?> GetTrustCensusStatuses(string ukprn);
        Task<CensusCollectionSummary?> GetSchoolCensusStatus(string laestab);
        Task<string> GetTrustCensusHeadline(List<string> laestabs);
    }

    public class CensusService(ICollectApiService collectApiService, IAcademiesApiService academiesApiService) : ICensusService
    {
        public async Task<CensusCollectionSummary?> GetSchoolCensusStatus(string laestab)
        {

            var censusSummary = await collectApiService.GetSubmissionSummary(GetLatestCensus(), laestab);
            return censusSummary;
        }

        public async Task<string> GetTrustCensusHeadline(List<string> laestabs)
        {
            if (laestabs.Any())
            {
                var censusDetails = await collectApiService.GetSubmissionSummaries(GetLatestCensus(), laestabs);

                if (censusDetails.Any())
                {
                    var totalApproved = censusDetails.Count(d => d.ReturnStatusCode == CollectApiService.ApprovedCode);
                    return $"{censusDetails.Count - totalApproved} of {censusDetails.Count} censuses are incomplete for {GetLatestCensus()}";
                }
            }
            return "No Census data available for this trust";
        }

        public async Task<TrustCensusStatus?> GetTrustCensusStatuses(string ukprn)
        {
            var trust = await academiesApiService.GetTrustDetails(ukprn);
            if (trust == null)
            {
                return null;
            }

            var collectionDetails = await collectApiService.GetCollectionDetails(GetLatestCensus());

            var trustLaestabs = trust.Establishments.Select(e => e.Laestab).ToList();

            var censusDetails =
                await collectApiService.GetSubmissionSummaries(GetLatestCensus(), trustLaestabs);

            List<CensusStatus> censusStatuses = BuildSummaryList(censusDetails, trust.Establishments);

            return new TrustCensusStatus()

            {
                TrustName = trust.GiasData?.GroupName ?? "Unknown Trust",
                TrustUkprn = trust.GiasData?.Ukprn ?? "Unknown UKPRN",
                CollectionName = collectionDetails?.DcName ?? "Unknown Collection",
                OpenDate = collectionDetails?.OpenDate,
                DueDate = collectionDetails?.DueDate,
                CensusStatuses = censusStatuses
            };
        }

        private List<CensusStatus> BuildSummaryList(List<CensusCollectionSummary> censusDetails, List<AcademyEstablishment> establishments)
        {
            var censusStatuses = censusDetails
                .Join(establishments,
                    census => census.Laestab,
                    establishment => establishment.Laestab,
                    (census, establishment) => new CensusStatus
                    {
                        SchoolName = census.SchoolName,
                        Laestab = census.Laestab,
                        Ukprn = establishment.Ukprn,
                        ReturnStatusCode = census.ReturnStatusCode,
                    })
                .ToList();

            return censusStatuses;
        }
        
        public string GetLatestCensus()
        {
            return "SchoolCensus 2025_Spring";
        }
    }
}


public class TrustCensusStatus
{
    public string TrustName { get; set; } = string.Empty;
    public string TrustUkprn { get; set; } = string.Empty;
    public string CollectionName { get; set; } = String.Empty;
    public DateTime? OpenDate { get; set; }
    public DateTime? DueDate { get; set; }
    public List<CensusStatus> CensusStatuses { get; set; } = new();
}

