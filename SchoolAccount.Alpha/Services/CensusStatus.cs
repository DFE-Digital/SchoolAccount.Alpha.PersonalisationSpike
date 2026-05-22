namespace SchoolAccount.Alpha.Services;

public class CensusStatus
{
    const string COMPLETE_STATUS = "Complete";
    const string IN_PROGRESS_STATUS = "In Progress";
    const string ACTION_REQUIRED_STATUS = "Action required";
    const string NOT_STARTED_STATUS = "Not Started";
    public string SchoolName { get; set; } = String.Empty;
    public string Laestab { get; set; } = String.Empty;
    public string Ukprn { get; set; } = String.Empty;
    public int ReturnStatusCode { get; set; }
    public string Status => GetSimpleStatus();
    public string PriorityTag => GetPriorityTag();

    private string GetPriorityTag()
    {
        if (Status == COMPLETE_STATUS)
        {
            return "green";
        }
        else if (Status == IN_PROGRESS_STATUS)
        {
            return "yellow";
        }
        else if (Status == ACTION_REQUIRED_STATUS)
        {
            return "red";
        }
        else
        {
            return "grey";
        }
    }

    private string GetSimpleStatus()
    {
        var inProgressCodes = new List<int> { 24, 12, 16, 17, 18, 19 };
        var actionNeededCodes = new List<int> { 5, 8, 11, 13, 23, 32 };
        if (ReturnStatusCode == 7)
        {
            return COMPLETE_STATUS;
        }
        else if (inProgressCodes.Contains(ReturnStatusCode))
        {
            return IN_PROGRESS_STATUS;
        }
        else if (actionNeededCodes.Contains(ReturnStatusCode))
        {
            return ACTION_REQUIRED_STATUS;
        }
        else
        {
            return NOT_STARTED_STATUS;
        }
    }
}