using SchoolAccount.Alpha.Services.Models;

namespace SchoolAccount.Alpha.ViewModels
{
    public class UserViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<DsiOrganisation> Organisations { get; set; } = new();
    }
}
