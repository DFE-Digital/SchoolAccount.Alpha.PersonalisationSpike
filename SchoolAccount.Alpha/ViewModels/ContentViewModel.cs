using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.ViewModels
{
    public class ContentViewModel
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public List<GovUkSearchResultItem> Results { get; set; } = new();
        public string PageTitle { get; set; } = string.Empty;
    }
}
