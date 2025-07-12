using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Controllers;

// Controlador responsável pelas páginas principais e autenticação do site
public class HomeController : Controller {
    // Logger para registo de eventos e erros
    private readonly ILogger<HomeController> _logger;

    // Construtor: injeta o logger
    public HomeController(ILogger<HomeController> logger) {
        _logger = logger;
    }

    // Página inicial
    public IActionResult Index() {
        return View();
    }

    // Página "Sobre"
    public IActionResult About() {
        return View();
    }

    // Página de privacidade
    public IActionResult Privacy() {
        return View();
    }

    // Página dedicada ao jogo Super Mario 64
    public IActionResult SuperMario64() {
        return View();
    }
    // Página dedicada ao jogo Minecraft
    public IActionResult Minecraft() {
        return View();
    }

     public IActionResult Celeste() {
        return View();
    }

     public IActionResult DarkSouls() {
        return View();
    }

     public IActionResult HollowKnight() {
        return View();
    }

    // Exibe o formulário de login
    [HttpGet]
    public IActionResult Login() {
        return View();
    }

    // Processa o login submetido
    [HttpPost]
    public IActionResult Login(string username, string password) {
        // Exemplo simples: login hardcoded
        if (username == "admin" && password == "admin") {
            // Autenticação fictícia, pode usar Session ou TempData
            TempData["User"] = username;
            return RedirectToAction("Index");
        }
        ViewBag.Error = "Credenciais inválidas.";
        return View();
    }

    // Exibe o formulário de registo
    [HttpGet]
    public IActionResult Registo() {
        return View();
    }

    // Processa o registo submetido
    [HttpPost]
    public IActionResult Registo(string newUsername, string newPassword) {
        // Exemplo simples: não persiste, apenas valida
        if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword)) {
            ViewBag.RegisterError = "Preencha todos os campos.";
            return View();
        }
        if (newUsername == "admin") {
            ViewBag.RegisterError = "Este username já existe.";
            return View();
        }
        ViewBag.RegisterSuccess = "Registro efetuado com sucesso! Faça login.";
        return View();
    }

    // Exibe o formulário de submissão de speedrun
    [HttpGet]
    public IActionResult Submit() {
        // Preenche listas de jogos e categorias para o formulário
        ViewBag.Games = new List<string> { "Super Mario 64", "Minecraft", "Celeste", "Hollow Knight", "Dark Souls" };
        ViewBag.Categories = new List<string> { "Any%", "100%", "Glitchless", "Low%", "Speedrun" };
        return View();
    }

    // Processa a submissão de speedrun
    [HttpPost]
    public IActionResult Submit(string GameName, string Category, string Time, string VideoUrl, string RunnerName, string Country, string Platform) {
        ViewBag.Games = new List<string> { "Super Mario 64", "Minecraft", "Celeste", "Hollow Knight", "Dark Souls" };
        ViewBag.Categories = new List<string> { "Any%", "100%", "Glitchless", "Low%", "Speedrun" };
        string error = null;
        // Valida o formato do tempo submetido
        if (!System.Text.RegularExpressions.Regex.IsMatch(Time ?? "", "^\\d{1,2}:\\d{2}:\\d{2}$")) {
            error = "Tempo inválido! Use o formato hh:mm:ss.";
        }
        if (error != null) {
            ViewBag.Error = error;
            return View();
        }
        ViewBag.Success = "Speedrun submetida com sucesso!";
        return View();
    }

    // Página de erro
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}