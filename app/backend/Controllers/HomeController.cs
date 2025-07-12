using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult SuperMario64()
    {
        return View();
    }
    public IActionResult Minecraft()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        // Exemplo simples: login hardcoded
        if (username == "admin" && password == "admin")
        {
            // Autenticação fictícia, pode usar Session ou TempData
            TempData["User"] = username;
            return RedirectToAction("Index");
        }
        ViewBag.Error = "Credenciais inválidas.";
        return View();
    }

    [HttpGet]
    public IActionResult Registo()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registo(string newUsername, string newPassword)
    {
        // Exemplo simples: não persiste, apenas valida
        if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
        {
            ViewBag.RegisterError = "Preencha todos os campos.";
            return View();
        }
        if (newUsername == "admin")
        {
            ViewBag.RegisterError = "Este username já existe.";
            return View();
        }
        ViewBag.RegisterSuccess = "Registro efetuado com sucesso! Faça login.";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
