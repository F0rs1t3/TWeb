using System.Diagnostics;
using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using TWeb.Models;

namespace TWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var carService = new CarService();
        var brands = carService.GetAllCarBrands();

        return View(brands); // Pass the list to the view
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
