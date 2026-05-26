using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.ViewModels
{
    public class SchoolViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string SchoolType { get; set; } = string.Empty;
        public string PhaseOfEducation { get; set; } = string.Empty;
        public string NumberOfPupils { get; set; } = string.Empty;
        public string PercentageEligibleForFsm { get; set; } = string.Empty;
        public string PercentageFsm { get; set; } = string.Empty;
        public CensusDetailsViewModel? CensusDetails { get; set; }
        public List<CollectionDetail> Censuses { get; set; } = new();

        public SchoolViewModel()
        {
        }

        public SchoolViewModel(SAUser user, AcademyOrganisation academyDetails, CensusSummary? censusSummary, List<CollectionDetail> censuses)
        {
            UserName = $"{user.GivenName} {user.LastName}";
            SchoolName = academyDetails.EstablishmentName;
            SchoolType = academyDetails.EstablishmentType?.Name ?? "Unknown";
            PhaseOfEducation = academyDetails.PhaseOfEducation?.Name ?? "Unknown";
            NumberOfPupils = academyDetails.Census?.NumberOfPupils ?? "Unknown";
            PercentageEligibleForFsm = academyDetails.Census?.PercentageEligableForFSM6Years ?? "Unknown";
            PercentageFsm = academyDetails.Census?.PercentageFsm ?? "Unknown";
            CensusDetails = censusSummary != null ? new CensusDetailsViewModel(censusSummary) : null;
            Censuses = censuses;
        }

    }
}