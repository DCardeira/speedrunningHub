using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.Models;
using System.Security.Claims;

namespace SpeedRunningHub.Controllers
{

    /// API Controller para operações relacionadas com jogos, imagens e speedruns.
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BlobServiceClient _blobService;


        /// Construtor do GamesController. Recebe contexto da base de dados e serviço de blobs Azure.

        public GamesController(AppDbContext context, BlobServiceClient blobService)
        {
            _context = context;
            _blobService = blobService;
        }


        /// Obtém todos os jogos, incluindo imagens associadas.

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            return await _context.Games
                .Include(g => g.GameImages)
                .ToListAsync();
        }


        /// Obtém um jogo específico por ID, incluindo speedruns aprovadas, guias aprovados e imagens.

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            var game = await _context.Games
                .Include(g => g.SpeedrunRecords.Where(r => r.IsApproved))
                .Include(g => g.Guides.Where(gd => gd.IsApproved))
                .Include(g => g.GameImages)
                .FirstOrDefaultAsync(g => g.GameId == id);

            if (game == null)
                return NotFound();

            return game;
        }


        /// Adiciona um novo jogo à base de dados. Apenas moderadores podem aceder.

        // POST: api/Games
        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<ActionResult<Game>> PostGame(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGame), new { id = game.GameId }, game);
        }


        /// Faz upload de uma imagem para um jogo específico. Apenas moderadores podem aceder.
        // POST: api/Games/5/images
        [HttpPost("{gameId}/images")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> UploadGameImage(int gameId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum ficheiro recebido.");

            var container = _blobService.GetBlobContainerClient("game-images");
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobName = $"{gameId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = container.GetBlobClient(blobName);

            await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

            var record = new GameImage
            {
                GameId = gameId,
                FileName = blobName,
                FilePath = blobClient.Uri.ToString(),
                UploadedAt = DateTime.UtcNow,
                UploadedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            _context.GameImages.Add(record);
            await _context.SaveChangesAsync();

            return Ok(new { record.GameImageId, record.FilePath });
        }
    }
}