using Azure.Identity;                           // Azure Key Vault auth
using Azure.Security.KeyVault.Secrets;           // Key Vault client
using Azure.Storage.Blobs;                       // Blob Storage client
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpeedRunningHub.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configurar Azure Key Vault como fonte de configuração ---
var kvUrl = builder.Configuration["KeyVault:Url"];
if (!string.IsNullOrEmpty(kvUrl)){
    builder.Configuration.AddAzureKeyVault(
        new Uri(kvUrl),
        new DefaultAzureCredential()
    );
}

// --- 2. Ler secrets do Key Vault (ou fallback para appsettings) ---
var connString = builder.Configuration["DefaultConnection"];
var jwtSecret   = builder.Configuration["JwtSettings:SecretKey"];
var blobConn    = builder.Configuration["BlobStorage:ConnectionString"];

// --- 3. Configurar EF Core com MySQL ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connString,
        new MySqlServerVersion(new Version(8, 0, 27))
    )
);

// --- 4. Configurar cliente Blob Storage ---
builder.Services.AddSingleton(_ => new BlobServiceClient(blobConn));

// --- 5. Configurar autenticação JWT ---
builder.Services.AddAuthentication(options =>{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>{
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience            = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// File: Controllers/GamesController.cs
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;

namespace SpeedRunningHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase{
        private readonly AppDbContext _context;
        private readonly BlobServiceClient _blobService;

        public GamesController(AppDbContext context, BlobServiceClient blobService){
            _context     = context;
            _blobService = blobService;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames(){
            return await _context.Games
                .Include(g => g.GameImages)
                .ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id){
            var game = await _context.Games
                .Include(g => g.SpeedrunRecords.Where(r => r.IsApproved))
                .Include(g => g.Guides.Where(gd => gd.IsApproved))
                .Include(g => g.GameImages)
                .FirstOrDefaultAsync(g => g.GameId == id);

            if (game == null)
                return NotFound();

            return game;
        }

        // POST: api/Games
        [HttpPost]
        // TODO: Adicionar [Authorize(Roles = "Moderator")] após configurar roles
        public async Task<ActionResult<Game>> PostGame(Game game){
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGame), new { id = game.GameId }, game);
        }

        // POST: api/Games/5/images
        [HttpPost("{gameId}/images")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> UploadGameImage(int gameId, IFormFile file){
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum ficheiro recebido.");

            var container = _blobService.GetBlobContainerClient("game-images");
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobName = $"{gameId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = container.GetBlobClient(blobName);

            await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

            var record = new GameImage{
                GameId           = gameId,
                FileName         = blobName,
                FilePath         = blobClient.Uri.ToString(),
                UploadedAt       = DateTime.UtcNow,
                UploadedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            _context.GameImages.Add(record);
            await _context.SaveChangesAsync();

            return Ok(new { record.GameImageId, record.FilePath });
        }
    }
}