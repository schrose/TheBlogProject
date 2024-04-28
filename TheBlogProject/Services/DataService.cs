using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;

namespace TheBlogProject.Services;

public class DataService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<BlogUser> _userManager;

    public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    
    public async Task ManageDataAsync()
    {
        await _dbContext.Database.MigrateAsync();
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (_dbContext.Roles.Any())
        {
            return;
        }
        foreach (var role in Enum.GetNames(typeof(BlogRole)))
        {
           await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    
    private async Task SeedUsersAsync()
    {
        if (_dbContext.Users.Any())
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
        await _userManager.CreateAsync(adminUser, "Abc&123!");
        await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());
        
        BlogUser modUser = new()
        {
            Email = "sean@seanschroder.com",
            UserName = "sean@seanschroder.com",
            FirstName = "Sean Mod",
            LastName = "Schroder",
            PhoneNumber = "(217) 274-4712",
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(modUser, "Abc&123!");
        await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
    }
}