using AccessManagementSystem_API.Data;
using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessManagementSystem_API.Services.Implementation
{
    public class RoleManagmentService : IRoleManagmentService
    {
        private readonly AppDbContext context;
        public RoleManagmentService(AppDbContext _context)
        {
            this.context = _context;
        }

        public async Task<APIResponse> AssignRolePermission(List<RolePermission> _data)
        {
            APIResponse response = new APIResponse();
            int processcount = 0;
            try
            {
                using (var dbtransaction = await this.context.Database.BeginTransactionAsync())
                {
                    if (_data.Count > 0)
                    {
                        _data.ForEach(item =>
                        {
                            var userdata = this.context.RolePermissions.FirstOrDefault(item1 => item1.Userrole == item.Userrole &&
                            item1.Menucode == item.Menucode);
                            if (userdata != null)
                            {
                                userdata.Haveview = item.Haveview;
                                userdata.Haveadd = item.Haveadd;
                                userdata.Havedelete = item.Havedelete;
                                userdata.Haveedit = item.Haveedit;
                                processcount++;
                            }
                            else
                            {
                                this.context.RolePermissions.Add(item);
                                processcount++;

                            }

                        });

                        if (_data.Count == processcount)
                        {
                            await this.context.SaveChangesAsync();
                            await dbtransaction.CommitAsync();
                            response.ResponseCode = 201;    
                            response.Result = "pass";
                            response.Message = "Saved successfully.";
                        }
                        else
                        {
                            await dbtransaction.RollbackAsync();
                        }

                    }
                    else
                    {
                        response.ResponseCode = 400;
                        response.Result = "fail";
                        response.Message = "Failed";
                    }
                }

            }
            catch (Exception ex)
            {
                response = new APIResponse();
            }

            return response;
        }

        public async Task<List<MenuDto>> GetAllMenuByRole(string userrole)
        {
            List<MenuDto> appmenus = new List<MenuDto>();

            var accessdata = (from menu in this.context.RolePermissions.Where(o => o.Userrole == userrole && o.Haveview)
                              join m in this.context.Menus on menu.Menucode equals m.Code into _jointable
                              from p in _jointable.DefaultIfEmpty()
                              select new { code = menu.Menucode, name = p.Name }).ToList();
            if (accessdata.Any())
            {
                accessdata.ForEach(item =>
                {
                    appmenus.Add(new MenuDto()
                    {
                        code = item.code,
                        Name = item.name
                    });
                });
            }

            return appmenus;
        }

        public async Task<List<Menu>> GetAllMenus()
        {
            return await this.context.Menus.ToListAsync();
        }

        public async Task<MenuPermissionDto> GetMenuPermissionByRole(string userrole, string menucode)
        {
            MenuPermissionDto menupermission = new MenuPermissionDto();
            var _data = await this.context.RolePermissions.FirstOrDefaultAsync(o => o.Userrole == userrole && o.Haveview
            && o.Menucode == menucode);
            if (_data != null)
            {
                menupermission.code = _data.Menucode;
                menupermission.Haveview = _data.Haveview;
                menupermission.Haveadd = _data.Haveadd;
                menupermission.Haveedit = _data.Haveedit;
                menupermission.Havedelete = _data.Havedelete;
            }
            return menupermission;
        }
    }
}
