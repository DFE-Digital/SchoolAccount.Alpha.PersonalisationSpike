using System.ComponentModel.DataAnnotations;

namespace SchoolAccount.Alpha.Services.Config
{
    public class GovUkSearchApiConfig
    {
        [Required]
        public string PublicUrl { get; set; } = string.Empty;
    }
}
