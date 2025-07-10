using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeedRunningHub.Data;
using SpeedRunningHub.DTOs;
using SpeedRunningHub.Models;
using System.Security.Claims;

namespace SpeedRunningHub.Controllers {
    [ApiController]
    [Route("api/games/{gameId}/guides")]
    public class GuidesController : ControllerBase {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GuidesController(AppDbContext context, IWebHostEnvironment env) {
            _context = context;
            _env = env;
        }

        // Obtém todos os guias aprovados para um jogo específico.
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
                .Include(g => g.Images)
                .FirstOrDefaultAsync(g => g.GameId == gameId && g.GuideId == guideId);

            if (guide == null) return NotFound();

            // Se o guia não estiver aprovado, apenas o autor ou um moderador o pode ver.
            if (!guide.IsApproved) {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                if (guide.UserId != currentUserId && !userRoles.Contains("Moderator")) {
                    return Forbid();
                }
            }

            return Ok(guide);
        }

        // Cria um novo guia. Apenas para utilizadores com papel "Runner" ou "Moderator".
        [HttpPost]
        [Authorize(Roles = "Runner,Moderator")]
        public async Task<ActionResult<Guide>> CreateGuide(int gameId, [FromBody] GuideCreateDto guideDto) {
            if (!await _context.Games.AnyAsync(g => g.GameId == gameId))
                return NotFound("Jogo não encontrado.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var newGuide = new Guide {
                GameId = gameId,
                UserId = userId,
                Title = guideDto.Title,
                Content = guideDto.Content,
                DateCreated = DateTime.UtcNow,
                IsApproved = false // Novos guias começam como não aprovados.
            };

            _context.Guides.Add(newGuide);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetGuideById", new { gameId = newGuide.GameId, guideId = newGuide.GuideId }, newGuide);
        }

        // Aprova um guia. Apenas para moderadores.
        [HttpPut("{guideId}/approve")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> ApproveGuide(int gameId, int guideId) {
            var guide = await _context.Guides.FirstOrDefaultAsync(g => g.GameId == gameId && g.GuideId == guideId);
            if (guide == null) return NotFound();

            guide.IsApproved = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Apaga um guia. Apenas para moderadores.
        [HttpDelete("{guideId}")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> DeleteGuide(int gameId, int guideId) {
            var guide = await _context.Guides.FirstOrDefaultAsync(g => g.GameId == gameId && g.GuideId == guideId);
            if (guide == null) return NotFound();

            _context.Guides.Remove(guide);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Endpoint para fazer upload de uma imagem para um guia.
        [HttpPost("{guideId}/images")]
        [Authorize(Roles = "Runner,Moderator")]
        public async Task<IActionResult> UploadGuideImage(int gameId, int guideId, IFormFile file) {
            var guide = await _context.Guides.FindAsync(guideId);
            if (guide == null || guide.GameId != gameId) return NotFound("Guia não encontrado.");

            // Verifica se o utilizador autenticado é o autor do guia.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (guide.UserId != userId) return Forbid();

            if (file == null || file.Length == 0) return BadRequest("Ficheiro não enviado.");

            var uploadsFolderPath = Path.Combine(_env.WebRootPath, "uploads", "guides");
            if (!Directory.Exists(uploadsFolderPath)) {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            var guideImage = new GuideImage {
                GuideId = guideId,
                ImagePath = $"/uploads/guides/{fileName}"
            };

            _context.GuideImages.Add(guideImage);
            await _context.SaveChangesAsync();

            return Ok(new { filePath = guideImage.ImagePath });
        }
    }
}