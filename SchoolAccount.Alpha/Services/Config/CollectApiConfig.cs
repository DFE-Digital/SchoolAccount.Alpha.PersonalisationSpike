using System.ComponentModel.DataAnnotations;

namespace SchoolAccount.Alpha.Services.Config
{
    public class CollectApiConfig
    {
        [Required]
        public string PublicUrl { get; set; } = string.Empty;
    }
}