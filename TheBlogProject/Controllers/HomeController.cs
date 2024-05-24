using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Models;
using TheBlogProject.Services;
using TheBlogProject.ViewModels;
using X.PagedList;

namespace TheBlogProject.Controllers;

public class HomeController(ApplicationDbContext context, IMailService mailService) : Controller
{
    public async Task<IActionResult> Index(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 5;

        var blogs = context.Blogs
            .Include(b => b.BlogUser)
            .OrderByDescending(b => b.Created)
            .ToPagedListAsync(pageNumber, pageSize);
        
        return View(await blogs);
    }
    
    public IActionResult About()
    {
        return View();
    }
    
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactMe model)
    {
        //This is where we will be emailing...
        model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
        await mailService.SendContactEmailAsync(model.Email, model.Name,model.Subject, model.Message);
        return RedirectToAction("Index");
    } 

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}