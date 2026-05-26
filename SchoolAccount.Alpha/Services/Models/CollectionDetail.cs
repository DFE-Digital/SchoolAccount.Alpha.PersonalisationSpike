using System.ComponentModel;

namespace SchoolAccount.Alpha.Services.Models;

public class CollectionDetail
{
    [Description("Data Collection ID")]
    public int DCID { get; set; }

    [Description("Data Collection Name")]
    public string DCName { get; set; } = string.Empty;

    [Description("Collection open date")]
    public DateTime? OpenDate { get; set; }

    [Description("Collection close date")]
    public DateTime? CloseDate { get; set; }

    [Description("Collection due date")]
    public DateTime? DueDate { get; set; }
}
