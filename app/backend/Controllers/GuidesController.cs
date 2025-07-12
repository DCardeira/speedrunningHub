using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.DTOs;
using SpeedRunningHub.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace SpeedRunningHub.Controllers {
    [ApiController]
    [Route("api/games/{gameId}/guides")]
    public class GuidesController : ControllerBase {
        // Dependências: contexto da base de dados e serviço de blobs Azure
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<NotificationHub> _hubContext;

        // Construtor: injeta dependências
        public GuidesController(AppDbContext context, BlobServiceClient blobService) {
            _context     = context;
            _blobService = blobService;
        }

        // Obtém todos os guias aprovados para um jogo específico
        // GET: api/games/5/guides
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guide>>> GetGuidesForGame(int gameId) {
            var guides = await _context.Guides
                .Where(g => g.GameId == gameId && g.IsApproved)
                .Include(g => g.User)
                .ToListAsync();
            return Ok(guides);
        }

        // Obtém um guia específico pelo seu ID.
        [HttpGet("{guideId:int}", Name = "GetGuideById")]
        public async Task<ActionResult<Guide>> GetGuide(int gameId, int guideId) {
            var guide = await _context.Guides
                .Include(g => g.User)
                .Include(g => g.GuideImages)
                .ToListAsync();
        }

        // Cria um novo guia para um jogo (apenas utilizadores com o papel 'Runner')
        // POST: api/games/5/guides
        [HttpPost]
        [Authorize(Roles = "Runner")]
        public async Task<ActionResult<Guide>> PostGuide(int gameId, Guide guide) {
            // Define propriedades do guia antes de guardar
            guide.GameId      = gameId;
            guide.DateCreated = DateTime.UtcNow;
            guide.IsApproved  = false;
            _context.Guides.Add(guide);
            await _context.SaveChangesAsync();
            // Retorna o guia criado
            return CreatedAtAction(nameof(GetGuides), new { gameId = gameId }, guide);
        }

        // Faz upload de uma imagem para um guia específico (apenas utilizadores com o papel 'Runner')
        // POST: api/games/5/guides/10/images
        [HttpPost("{guideId}/images")]
        [Authorize(Roles = "Runner,Moderator")]
        public async Task<IActionResult> UploadGuideImage(int gameId, int guideId, IFormFile file) {
            // Valida se o ficheiro foi recebido
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum ficheiro recebido.");

            // Obtém o container de blobs e cria se não existir
            var container = _blobService.GetBlobContainerClient("guide-images");
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Gera nome único para o blob
            var blobName = $"{guideId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var blobClient = container.GetBlobClient(blobName);

            // Faz upload do ficheiro para o Azure Blob Storage
            await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

            // Regista a imagem na base de dados
            var record = new GuideImage {
                GuideId          = guideId,
                FileName         = blobName,
                FilePath         = blobClient.Uri.ToString(),
                UploadedAt       = DateTime.UtcNow,
                UploadedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            _context.GuideImages.Add(record);
            await _context.SaveChangesAsync();

            // Retorna o id e caminho da imagem
            return Ok(new { record.GuideImageId, record.FilePath });
        }
    }
}