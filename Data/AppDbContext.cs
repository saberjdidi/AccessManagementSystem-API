using AccessManagementSystem_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccessManagementSystem_API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Insert Data in DataBase
          /*  modelBuilder.Entity<Menu>().HasData(
              new Menu
              {
                  Code = "customer",
                  Name = "Customer",
                  Status = true
              },
              new Menu
              {
                  Code = "user",
                  Name = "Users",
                  Status = true
              },
              new Menu
              {
                  Code = "role",
                  Name = "Roles",
                  Status = true
              }
              );

            modelBuilder.Entity<RolePermission>().HasData(
              new RolePermission
              {
                  Userrole = "Admin",
                  Menucode = "user",
                  Haveview = true,
                  Haveadd = true,
                  Havedelete = true,
                  Haveedit = true,
              },
              new RolePermission
              {
                  Userrole = "Admin",
                  Menucode = "customer",
                  Haveview = true,
                  Haveadd = true,
                  Havedelete = true,
                  Haveedit = true,
              },
              new RolePermission
              {
                  Userrole = "User",
                  Menucode = "customer",
                  Haveview = true,
                  Haveadd = true,
                  Havedelete = false,
                  Haveedit = false,
              }
              ); */
        }
    }
  
}
