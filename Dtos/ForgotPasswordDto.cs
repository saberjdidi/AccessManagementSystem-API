using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem_API.Dtos
{
    public class ForgotPasswordDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
