using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem_API.Dtos
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; } = null!;
    }
}
