using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TheBlogProject.Models;
using TheBlogProject.Services;
using TheBlogProject.ViewModels;

namespace TheBlogProject.Controllers;

public class HomeController(ILogger<HomeController> logger, IMailService mailService) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IMailService _mailService = mailService;

    public IActionResult Index()
    {
        return View();
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
        await _mailService.SendContactEmailAsync(model.Email, model.Name,model.Subject, model.Message);
        return RedirectToAction("Index");
    } 

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}