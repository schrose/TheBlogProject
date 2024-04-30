using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;

namespace TheBlogProject.Services;

public class DataService(
    ApplicationDbContext dbContext,
    RoleManager<IdentityRole> roleManager,
    UserManager<BlogUser> userManager)
{
    public async Task ManageDataAsync()
    {
        await dbContext.Database.MigrateAsync();
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (dbContext.Roles.Any())
        {
            return;
        }
        foreach (var role in Enum.GetNames(typeof(BlogRole)))
        {
           await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    
    private async Task SeedUsersAsync()
    {
        if (dbContext.Users.Any())
        {
            return;
        }
        BlogUser adminUser = new()
        {
            Email = "schrose@gmail.com",
            UserName = "schrose@gmail.com",
            FirstName = "Sean Admin",
            LastName = "Schroder",
            PhoneNumber = "(217) 274-4712",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, Guid.NewGuid().ToString());
        await userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());
        
        BlogUser modUser = new()
        {
            Email = "sean@seanschroder.com",
            UserName = "sean@seanschroder.com",
            FirstName = "Sean Mod",
            LastName = "Schroder",
            PhoneNumber = "(217) 274-4712",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(modUser, Guid.NewGuid().ToString());
        await userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
    }
}