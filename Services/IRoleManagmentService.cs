using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;

namespace AccessManagementSystem_API.Services
{
    public interface IRoleManagmentService
    {
        Task<APIResponse> AssignRolePermission(List<RolePermission> _data);
        Task<List<Menu>> GetAllMenus();
        Task<List<MenuDto>> GetAllMenuByRole(string userrole);
        Task<MenuPermissionDto> GetMenuPermissionByRole(string userrole, string menucode);
    }
}
