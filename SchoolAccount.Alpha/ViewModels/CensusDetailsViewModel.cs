using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SchoolAccount.Alpha.ViewModels
{
    public class CensusDetailsViewModel
    
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Errors { get; set; }
        public int Queries { get; set; }
        public int OkdErrorsQueries { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? AuthorisedDate { get; set; }
    }
}
