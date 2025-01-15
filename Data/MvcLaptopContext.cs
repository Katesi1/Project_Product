using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Models;

namespace MvcLaptop.Data
{
    public class MvcLaptopContext : IdentityDbContext<User, Role, string>
    {
        public MvcLaptopContext(DbContextOptions<MvcLaptopContext> options) : base(options)
        {
        }
        public DbSet<Product> Product { get; set; } = default!;
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Category>? Category { get; set; }
        public DbSet<ProductImage>? ProductImage { get; set; }
        public DbSet<OrderDetail>? OrderDetail { get; set; }
        public DbSet<Function> Functions { set; get; } = default!;
        public DbSet<Command> Commands { set; get; } = default!;
        public DbSet<Permission> Permissions { set; get; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().ToTable("Roles").Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            modelBuilder.Entity<User>().ToTable("Users").Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<ProductImage>().ToTable("ProductImage");
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);
            modelBuilder.Entity<Permission>()
                .HasKey(c => new { c.RoleId, c.FunctionId, c.CommandId });

            modelBuilder.Entity<Order>()
            .HasOne(e => e.User)
            .WithMany(e => e.Orders)
            .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<OrderDetail>()
            .HasOne(e => e.Order)
            .WithMany(e => e.orderDetails)
            .HasForeignKey(e => e.OrderId);

            // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            // {
            //     var tableName = entityType.GetTableName();
            //     if (tableName!.StartsWith("AspNet"))
            //     {
            //         entityType.SetTableName(tableName.Substring(6));
            //     }
            // }
        }



    }
}
