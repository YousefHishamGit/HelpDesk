using HelpDesk.Domain.Entity;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            IPasswordHasher<User> hasher,
            IConfiguration config)
        {
            // 1) Categories
            if (!await context.Categories.AnyAsync())
            {
                context.Categories.AddRange(
                    new Category { Name = "Hardware" },
                    new Category { Name = "Software" },
                    new Category { Name = "Network" },
                    new Category { Name = "Access & Accounts" }
                );
            }


            if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                // 2) Admin
                var admin = new User
                {
                    FullName = "System Admin",
                    Email = "admin@example.com",
                    Role = UserRole.Admin,
                    IsActive = true
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
                context.Users.Add(admin);
                //3) Agent
                var Agent = new User
                {
                    FullName = "System Agent",
                    Email = "agent@example.com",
                    Role = UserRole.Agent,
                    IsActive = true
                };
                Agent.PasswordHash = hasher.HashPassword(admin, "Agent@123");
                context.Users.Add(Agent);
                //3) Employee
                var Emp = new User
                {
                    FullName = "System Employee",
                    Email = "employee@example.com",
                    Role = UserRole.Employee,
                    IsActive = true
                };
                Emp.PasswordHash = hasher.HashPassword(admin, "Employee@123");
                context.Users.Add(Emp);


                await context.SaveChangesAsync();
            }
        }
    }
}
