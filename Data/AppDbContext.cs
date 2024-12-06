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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("tbl_category");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.CreatedAt).IsRequired();
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("tbl_product");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.ImagePath).IsRequired();
                entity.Property(p => p.CreatedAt).IsRequired();
                entity.HasOne(p => p.Category);
                      //.WithMany(c => c.Products)
                      //.HasForeignKey(p => p.CategoryId)
                      //.OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete
            });

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
