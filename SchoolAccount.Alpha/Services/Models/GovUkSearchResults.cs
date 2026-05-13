namespace SchoolAccount.Alpha.Services.Models;

public class GovUkSearchResults
{
    public int Total { get; set; }
    public int Start { get; set; }
    public List<GovUkSearchResultItem> Results { get; set; } = new ();
}