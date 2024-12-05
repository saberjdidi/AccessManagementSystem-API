using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;

namespace AccessManagementSystem_API.Services
{
    public interface IUserService
    {
        Task<APIResponse> UpdateStatus(UpdateRoleStatus updateStatus);
        Task<APIResponse> UpdateRole(UpdateRoleStatus updateRole);
        Task<ResponseDto> GetUserByEmail(string email);
    }
}
