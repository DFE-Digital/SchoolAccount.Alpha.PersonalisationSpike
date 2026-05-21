using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.ViewModels
{
    public class CensusDetailsViewModel

    {
        public string Name { get; set; } = string.Empty;
        public string? SchoolName { get; set; } = String.Empty;
        public string Status { get; set; } = string.Empty;
        public int Errors { get; set; }
        public int Queries { get; set; }
        public int OkdErrorsQueries { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? AuthorisedDate { get; set; }

        public CensusDetailsViewModel()
        {
        }

        public CensusDetailsViewModel(CensusCollectionSummary summary)
        {
            Name = summary.Collection;
            SchoolName = summary.SchoolName;
            Status = summary.ReturnStatus;
            Errors = summary.Errors;
            Queries = summary.Queries;
            OkdErrorsQueries = summary.OkdErrorsQueries;
            SubmittedDate = summary.SubmittedDate;
            ApprovedDate = summary.ApprovedDate;
            AuthorisedDate = summary.AuthorisedDate;
        }
    }
}
