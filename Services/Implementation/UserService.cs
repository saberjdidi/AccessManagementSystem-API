using AccessManagementSystem_API.Data;
using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccessManagementSystem_API.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(AppDbContext learndata, IMapper mapper, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = learndata;
            this.mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<APIResponse> UpdateRole(UpdateRoleStatus updateRole)
        {
            APIResponse response = new APIResponse();
            IdentityResult result = new IdentityResult();

            //var user = await _userManager.FindByEmailAsync(email);
            var user = await this.context.Users.FirstOrDefaultAsync(item => item.Email == updateRole.email && item.LockoutEnabled == true);
            if (user is null)
            {
                response.ResponseCode = 400;
                response.Result = "fail";
                response.Message = "User not found";
            }

            ///Many Role
            if (updateRole.Roles == null || !updateRole.Roles.Any())
            {
                response.ResponseCode = 400;
                response.Result = "fail";
                response.Message = "No roles provided";
                return response;
            }

            foreach (var item in updateRole.Roles!)
            {
                var _role = await _roleManager.FindByNameAsync(item);
                if (_role is null)
                {
                    response.ResponseCode = 400;
                    response.Result = "fail";
                    response.Message = "Role not found";
                }
                result = await _userManager.AddToRoleAsync(user!, _role!.Name!);
            }

            ///One Role
            //var role = await _roleManager.FindByNameAsync(updateRole.role);    
            //if (role is null)
            //{
            //    response.ResponseCode = 400;
            //    response.Result = "fail";
            //    response.Message = "Role not found";
            //}
            // result = await _userManager.AddToRoleAsync(user, role!.Name!);
            
            await this.context.SaveChangesAsync();
            if (result.Succeeded)
            {
                response.ResponseCode = 200;
                response.Result = "pass";
                response.Message = "Role assigned successfully";
            }

            //var _user = await this.context.Users.FirstOrDefaultAsync(item => item.Email == email && item.LockoutEnabled == true);
            //if (_user != null)
            //{
            //    _user.Role = userrole;
            //    await this.context.SaveChangesAsync();
            //    response.Result = "pass";
            //    response.Message = "User Role changed";
            //}
            //else
            //{
            //    response.Result = "fail";
            //    response.Message = "Invalid User";
            //}
            return response;
        }

        public async Task<APIResponse> UpdateStatus(UpdateRoleStatus updateStatus)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.Users.FirstOrDefaultAsync(item => item.Email == updateStatus.email);
            if (_user != null)
            {
                _user.LockoutEnabled = updateStatus.status;
                await this.context.SaveChangesAsync();
                response.ResponseCode = 200;
                response.Result = "pass";
                response.Message = "User Status changed";
            }
            else
            {
                response.ResponseCode = 400;
                response.Result = "fail";
                response.Message = "Invalid User";
            }
            return response;
        }

        public async Task<ResponseDto> GetUserByEmail(string email)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(user => user.Email == email);
            ResponseDto responseDto = new ResponseDto();

            if (user != null)
            {
                var roles = _userManager.GetRolesAsync(user).Result.ToArray();

                UserDetailDto userDetailDto = new UserDetailDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    AccessFailedCount = user.AccessFailedCount,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = roles
                };

                responseDto.StatusCode = 200;
                responseDto.IsSuccess = true;
                responseDto.Message = "Data Success";
                responseDto.Result = userDetailDto;
            }
            else
            {
                responseDto.StatusCode = 400;
                responseDto.IsSuccess = false;
                responseDto.Message = "Data Not Found";
                responseDto.Result = null!;
            }

            return responseDto;
        }
    }
}
